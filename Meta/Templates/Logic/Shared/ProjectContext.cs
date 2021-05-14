using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Hopper.Shared.Attributes;
using System.IO;
using Hopper.Meta.Stats;
using System;

namespace Hopper.Meta
{
    public class HopperProject
    {

    }

    public class GenerationEnvironment
    {
        public Solution _solution;
        public Project _project;
        public AutogenPaths paths;
        public Compilation _compilation;

        // TODO: This will be annoying to use if two mods defined two components with the same name
        // Even if one mod does not reference the other, duplicate names will cause collision.
        // So this dictionary has to be adjusted based on which assemblies are referenced by the given mod.
        // This problem is a long way away, though, so I'll leave it as it is right now.
        public Dictionary<string, ComponentSymbolWrapper> components;

        // Same problem here.
        public HashSet<string> aliases;

        private INamespaceSymbol _rootNamespace;
        public string RootNamespaceName => _project.AssemblyName;
        public ParsingContext statParsingContext;

        public bool TryAddComponent(ComponentSymbolWrapper wrapper)
        {
            if (components.ContainsKey(wrapper.ClassName))
            {
                ReportError($"The behavior {wrapper.ClassName} has been defined twice, which is not allowed.");
                return false;
            }

            components.Add(wrapper.ClassName, wrapper);
            return true;
        }

        public ErrorContext errorContext;
        public void ReportError(string errorMessage) => errorContext.Report(errorMessage);
        
        public T DoScoped<T>(IThing scopedThing, Func<T> func)
        {
            errorContext.PushThing(scopedThing);
            T result = func();
            errorContext.PopThing();
            return result;
        } 

        public GenerationEnvironment(string[] projectPaths)
        {
            aliases = new HashSet<string>();
            components = new Dictionary<string, ComponentSymbolWrapper>();
            statParsingContext = new ParsingContext("Hopper");
            errorContext = new ErrorContext();

            paths = new AutogenPaths();
            foreach (var p in projectPaths)
            {
                paths.Reset(Path.GetDirectoryName(p));
                paths.CreateOrEmpty();
            }
        }

        public async Task Reset(Project project)
        {
            _project = project;
            _solution = _project.Solution;
            _compilation = await project.GetCompilationAsync();
            RelevantSymbols.TryInitializeSingleton(_compilation);
            paths.Reset(Path.GetDirectoryName(project.FilePath));
            this._rootNamespace = GetRootNamespace();

            // Hopper.
            statParsingContext.ResetToRootScope();
            // Core.
            foreach (var scopeName in _project.AssemblyName.Split('.').Skip(1))
            {
                statParsingContext.PushScope(scopeName);
            }
            // Stat
            statParsingContext.PushScope("Stat");
        }

        public INamespaceSymbol GetRootNamespace()
        {
            var paths = _project.AssemblyName.Split('.');

            INamespaceSymbol result = _compilation.GlobalNamespace;
            foreach (var path in paths)
            {
                result = result.GetNamespaceMembers().Where(ns => ns.Name == path).Single();
            }

            return result;
        }

        public async Task<IEnumerable<INamedTypeSymbol>> FindAllDirectComponents()
        {
            return (await SymbolFinder.FindImplementationsAsync(
                RelevantSymbols.IComponent, _solution, transitive: false, null
            )).Where(s => s.IsContainedInNamespace(_rootNamespace));
        }

        public async Task<IEnumerable<INamedTypeSymbol>> FindAllTags()
        {
            return (await SymbolFinder.FindImplementationsAsync(
                RelevantSymbols.ITag, _solution, transitive: false, null
            )).Where(s => s.IsContainedInNamespace(_rootNamespace));
        }

        public async Task<IEnumerable<INamedTypeSymbol>> FindAllBehaviors()
        {
            return (await SymbolFinder.FindImplementationsAsync(
                RelevantSymbols.IBehavior, _solution, transitive: false, null
            )).Where(s => s.IsContainedInNamespace(_rootNamespace));
        }

        public IEnumerable<INamedTypeSymbol> GetNotNestedTypes() =>
            GetNotNestedTypes(_rootNamespace);

        public IEnumerable<INamedTypeSymbol> GetNotNestedTypes(INamespaceSymbol @namespace)
        {
            foreach (var type in @namespace.GetTypeMembers())
                yield return type;

            foreach (var nestedNamespace in @namespace.GetNamespaceMembers())
            foreach (var type in GetNotNestedTypes(nestedNamespace))
                yield return type;
        }

        public IEnumerable<ExportedMethodsClassSymbolWrapper> GetExportedMethodClasses()
        {
            var typeSymbols = GetNotNestedTypes();

            foreach (var typeSymbol in typeSymbols)
            {
                if (typeSymbol.IsStatic || typeSymbol.HasAttribute(RelevantSymbols.InstanceExportAttribute.symbol))
                {
                    var classWrapper = new ExportedMethodsClassSymbolWrapper(typeSymbol);
                    if (classWrapper.TryInit(this))
                    {
                        yield return classWrapper;
                    }
                }
            }
        }

        public IEnumerable<EntityTypeWrapper> GetEntityTypes()
        {
            var typeSymbols = GetNotNestedTypes();

            foreach (var typeSymbol in typeSymbols)
            {
                if (typeSymbol.IsStatic 
                    && typeSymbol.TryGetAttribute(RelevantSymbols.EntityTypeAttribute, out var a)
                    && a.Abstract == false)
                {
                    var wrapper = new EntityTypeWrapper(typeSymbol);
                    wrapper.TryInit(this);
                    yield return wrapper;
                }
            }
        }

        public IEnumerable<IFieldSymbol> GetAllFields() => GetAllFields(_rootNamespace);

        public IEnumerable<IFieldSymbol> GetAllFields(INamespaceOrTypeSymbol symbol)
        {
            foreach (var member in symbol.GetMembers())
            {
                if (member is IFieldSymbol field)
                {
                    yield return field;
                }
                else if (member is INamespaceOrTypeSymbol nested)
                {
                    foreach (var f in GetAllFields(nested))
                    {
                        yield return f;
                    }
                }
            }
        }

        public IEnumerable<FieldSymbolWrapper> GetStaticFieldsWithAttibute(INamedTypeSymbol attribute)
        {
            foreach (var field in GetAllFields())
            {
                if (field.IsStatic && field.HasAttribute(attribute))
                {
                    yield return new FieldSymbolWrapper(field);
                }
            }
        }

        public IEnumerable<SlotSymbolWrapper> GetSlots()
        {
            foreach (var field in GetAllFields())
            {
                if (field.IsStatic && field.TryGetAttribute(RelevantSymbols.SlotAttribute, out var attribute))
                {
                    yield return new SlotSymbolWrapper(field, attribute);
                }
            }
        }

        public IEnumerable<FieldSymbolWrapper> GetMethodClassInstances() =>
            GetStaticFieldsWithAttibute(RelevantSymbols.InstanceExportAttribute.symbol);

        public IEnumerable<FieldSymbolWrapper> GetFieldsRequiringInit() =>
            GetStaticFieldsWithAttibute(RelevantSymbols.RequiringInitAttribute.symbol);

        public IEnumerable<FieldSymbolWrapper> GetStaticIdentifyingStatFields() =>
            GetStaticFieldsWithAttibute(RelevantSymbols.IdentifyingStatAttribute.symbol);

        public IEnumerable<INamedTypeSymbol> GetFlagEnums()
        {
            foreach (var type in GetNotNestedTypes())
            {
                if (type.HasAttribute(RelevantSymbols.FlagsAttribute.symbol))
                {
                    yield return type;
                }
            }
        }
    }
}