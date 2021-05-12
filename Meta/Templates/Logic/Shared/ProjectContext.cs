using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Hopper.Shared.Attributes;
using System.IO;
using Hopper.Meta.Stats;

namespace Hopper.Meta
{
    public class HopperProject
    {

    }

    public class GlobalContext
    {
        public Solution _solution;
        public Project _project;
        public AutogenPaths _paths;
        public HashSet<Project> projectSet;
        public Compilation compilation;

        // TODO: This will be annoying to use if two mods defined two components with the same name
        // Even if one mod does not reference the other, duplicate names will cause collision.
        // So this dictionary has to be adjusted based on which assemblies are referenced by the given mod.
        // This problem is a long way away, though, so I'll leave it as it is right now.
        public Dictionary<string, ComponentSymbolWrapper> globalComponents;
        // Same problem here.
        public HashSet<string> globalAliases;
        public INamespaceSymbol rootNamespace;
        public string RootNamespaceName => _project.AssemblyName;
        public ParsingContext statParsingContext;


        public GlobalContext(string[] projectPaths)
        {
            globalAliases = new HashSet<string>();
            globalComponents = new Dictionary<string, ComponentSymbolWrapper>();
            statParsingContext = new ParsingContext("Hopper");

            _paths = new AutogenPaths();
            foreach (var p in projectPaths)
            {
                _paths.Reset(Path.GetDirectoryName(p));
                _paths.CreateOrEmpty();
            }
            
        }
        
        public async Task Reset(Project project)
        {
            _project = project;
            _solution = _project.Solution;
            projectSet = new HashSet<Project>{project};
            compilation = await project.GetCompilationAsync();
            RelevantSymbols.TryInitializeSingleton(compilation);
            _paths.Reset(Path.GetDirectoryName(project.FilePath));
            this.rootNamespace = GetRootNamespace();

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

            INamespaceSymbol result = compilation.GlobalNamespace;
            foreach (var path in paths)
            {
                result = result.GetNamespaceMembers().Where(ns => ns.Name == path).Single();
            }

            return result;
        }

        public async Task<IEnumerable<INamedTypeSymbol>> FindAllDirectComponents()
        {
            return (await SymbolFinder.FindImplementationsAsync(
                RelevantSymbols.icomponent, _solution, transitive: false, projectSet.ToImmutableHashSet()
            )).Where(s => s.IsContainedInNamespace(rootNamespace));
        }

        public async Task<IEnumerable<INamedTypeSymbol>> FindAllTags()
        {
            return (await SymbolFinder.FindImplementationsAsync(
                RelevantSymbols.itag, _solution, transitive: false, projectSet.ToImmutableHashSet()
            )).Where(s => s.IsContainedInNamespace(rootNamespace));
        }

        public async Task<IEnumerable<INamedTypeSymbol>> FindAllBehaviors()
        {
            return (await SymbolFinder.FindImplementationsAsync(
                RelevantSymbols.ibehavior, _solution, transitive: false, projectSet.ToImmutableHashSet()
            )).Where(s => s.IsContainedInNamespace(rootNamespace));
        }

        public IEnumerable<INamedTypeSymbol> GetNotNestedTypes() =>
            GetNotNestedTypes(rootNamespace);

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
                if (typeSymbol.IsStatic || typeSymbol.HasAttribute(RelevantSymbols.instanceExportAttribute))
                {
                    var classWrapper = new ExportedMethodsClassSymbolWrapper(typeSymbol);
                    classWrapper.Init(this);
                    if (classWrapper.ShouldGenerate())
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
                    && typeSymbol.TryGetAttribute(RelevantSymbols.entityTypeAttribute, out var a)
                    && a.MapToType<EntityTypeAttribute>().Abstract == false)
                {
                    var wrapper = new EntityTypeWrapper(typeSymbol);
                    wrapper.InitWithErrorHandling(this);
                    yield return wrapper;
                }
            }
        }

        public IEnumerable<IFieldSymbol> GetAllFields() => GetAllFields(rootNamespace);

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
                if (field.IsStatic && field.TryGetAttribute(RelevantSymbols.slotAttribute, out var attribute))
                {
                    yield return new SlotSymbolWrapper(field, attribute.MapToType<SlotAttribute>());
                }
            }
        }



        public IEnumerable<FieldSymbolWrapper> GetMethodClassInstances() =>
            GetStaticFieldsWithAttibute(RelevantSymbols.instanceExportAttribute);

        public IEnumerable<FieldSymbolWrapper> GetFieldsRequiringInit() =>
            GetStaticFieldsWithAttibute(RelevantSymbols.requiringInitAttribute);

        public IEnumerable<FieldSymbolWrapper> GetStaticIdentifyingStatFields() =>
            GetStaticFieldsWithAttibute(RelevantSymbols.identifyingStatAttribute);

        public IEnumerable<FlagEnumSymbolWrapper> GetFlagEnums()
        {
            foreach (var type in GetNotNestedTypes())
            {
                if (type.HasAttribute(RelevantSymbols.flagsAttribute))
                {
                    var wrapper = new FlagEnumSymbolWrapper(type);
                    if (wrapper.InitWithErrorHandling(this)) yield return wrapper;
                }
            }
        }
    }
}