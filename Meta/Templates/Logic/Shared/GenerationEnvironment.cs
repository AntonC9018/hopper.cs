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
    public class GenerationEnvironment
    {
        private Project _project;
        private Compilation _compilation;

        public Solution Solution { get; private set; }
        public AutogenPaths Paths { get; private set; }

        // TODO: This will be annoying to use if two mods defined two components with the same name
        // Even if one mod does not reference the other, duplicate names will cause collision.
        // So this dictionary has to be adjusted based on which assemblies are referenced by the given mod.
        // This problem is a long way away, though, so I'll leave it as it is right now.
        public Dictionary<string, TypeSymbolWrapperBase> exportingClasses;
        public Dictionary<INamedTypeSymbol, ContextSymbolWrapper> contexts;
        public Dictionary<string, IChainWrapper> chains; 
        public HashSet<string> aliases;

        private INamespaceSymbol _rootNamespace;
        public string RootNamespaceName => _project.AssemblyName;
        public ParsingContext statParsingContext;

        public GenerationEnvironment(string[] projectPaths)
        {
            aliases            = new HashSet<string>();
            exportingClasses   = new Dictionary<string, TypeSymbolWrapperBase>();
            contexts           = new Dictionary<INamedTypeSymbol, ContextSymbolWrapper>();
            statParsingContext = new ParsingContext("Hopper");
            chains             = new Dictionary<string, IChainWrapper>();
            errorContext       = new ErrorContext();

            Paths = new AutogenPaths();
            foreach (var p in projectPaths)
            {
                Paths.Reset(Path.GetDirectoryName(p));
                Paths.CreateOrEmpty();
            }
        }

        public bool TryAddExportingClass(TypeSymbolWrapperBase wrapper)
        {
            if (exportingClasses.ContainsKey(wrapper.ClassName))
            {
                ReportError($"The exporting class {wrapper.ClassName} has been defined twice, which is not allowed.");
                return false;
            }

            exportingClasses.Add(wrapper.ClassName, wrapper);
            return true;
        }

        public bool TryCacheContext(INamedTypeSymbol contextSymbol, out ContextSymbolWrapper context)
        {
            context = new ContextSymbolWrapper(contextSymbol);
            return context.TryInit(this);
        }

        public bool TryGetContextLazy(INamedTypeSymbol contextSymbol, out ContextSymbolWrapper context)
        {
            if (!contexts.TryGetValue(contextSymbol, out context))
            {
                return TryCacheContext(contextSymbol, out context);
            }
            return true;
        }

        public bool TryAddChain(IChainWrapper chain)
        {
            string uid = chain.GetUid();

            if (chains.ContainsKey(uid)) return false;

            chains.Add(uid, chain);
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

        public async Task Reset(Project project)
        {
            _project = project;
            Solution = _project.Solution;
            _compilation = await project.GetCompilationAsync();
            RelevantSymbols.TryInitializeSingleton(_compilation);
            Paths.Reset(Path.GetDirectoryName(project.FilePath));
            _rootNamespace = GetRootNamespace();

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
                RelevantSymbols.IComponent, Solution, transitive: false, null
            )).Where(s => s.IsContainedInNamespace(_rootNamespace));
        }

        public async Task<IEnumerable<INamedTypeSymbol>> FindAllTags()
        {
            return (await SymbolFinder.FindImplementationsAsync(
                RelevantSymbols.ITag, Solution, transitive: false, null
            )).Where(s => s.IsContainedInNamespace(_rootNamespace));
        }

        public async Task<IEnumerable<INamedTypeSymbol>> FindAllBehaviors()
        {
            return (await SymbolFinder.FindImplementationsAsync(
                RelevantSymbols.IBehavior, Solution, transitive: false, null
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

        public IEnumerable<ExportedStuffClassSymbolWrapper> GetOtherExportingClasses()
        {
            var typeSymbols = GetNotNestedTypes();

            foreach (var typeSymbol in typeSymbols)
            {
                if (typeSymbol.IsStatic 
                    || typeSymbol.HasAttribute(RelevantSymbols.InstanceExportAttribute.symbol)
                    && !exportingClasses.ContainsKey(typeSymbol.Name))
                {
                    var classWrapper = new ExportedStuffClassSymbolWrapper(typeSymbol);
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