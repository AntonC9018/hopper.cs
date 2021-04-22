using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Hopper.Shared.Attributes;

namespace Hopper.Meta
{
    public class ProjectContext
    {
        public Solution _solution;
        public Project _project;
        public HashSet<Project> projectSet;
        public Compilation compilation;

        // TODO: This will be annoying to use if two mods defined two components with the same name
        // Even if one mod does not reference the other, duplicate names will cause collision.
        // So this dictionary has to be adjusted based on which assemblies are referenced by the given mod.
        // This problem is a long way away, though, so I'll leave it as it is right now.
        public Dictionary<string, ComponentSymbolWrapper> globalComponents;
        // Same problem here.
        public HashSet<string> globalAliases;


        public ProjectContext(Solution solution)
        {
            _solution = solution;
            globalAliases = new HashSet<string>();
            globalComponents = new Dictionary<string, ComponentSymbolWrapper>();
        }
        
        public async Task Reset(Project project)
        {
            _project = project;
            projectSet = new HashSet<Project>{project};
            compilation = await project.GetCompilationAsync();
            RelevantSymbols.TryInitializeSingleton(compilation);
        }

        public Task<IEnumerable<INamedTypeSymbol>> FindAllDirectComponents()
        {
            return SymbolFinder.FindImplementationsAsync(
                RelevantSymbols.Instance.icomponent, _solution, transitive: false, projectSet.ToImmutableHashSet()
            );
        }

        public Task<IEnumerable<INamedTypeSymbol>> FindAllTags()
        {
            return SymbolFinder.FindImplementationsAsync(
                RelevantSymbols.Instance.itag, _solution, transitive: false, projectSet.ToImmutableHashSet()
            );
        }

        public Task<IEnumerable<INamedTypeSymbol>> FindAllBehaviors()
        {
            return SymbolFinder.FindImplementationsAsync(
                RelevantSymbols.Instance.ibehavior, _solution, transitive: false, projectSet.ToImmutableHashSet()
            );
        }
        public IEnumerable<INamedTypeSymbol> GetNotNestedTypes() =>
            GetNotNestedTypes(compilation.GlobalNamespace);

        public IEnumerable<INamedTypeSymbol> GetNotNestedTypes(INamespaceSymbol @namespace)
        {
            foreach (var type in @namespace.GetTypeMembers())
                yield return type;

            foreach (var nestedNamespace in @namespace.GetNamespaceMembers())
            foreach (var type in GetNotNestedTypes(nestedNamespace))
                yield return type;
        }

        public IEnumerable<StaticClassSymbolWrapper> GetStaticClassesWithExportedMethods()
        {
            var typeSymbols = GetNotNestedTypes();

            foreach (var typeSymbol in typeSymbols)
            {
                if (typeSymbol.IsStatic)
                {
                    var classWrapper = new StaticClassSymbolWrapper(typeSymbol);
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
                    && typeSymbol.GetAttributes().Any(
                        a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, RelevantSymbols.Instance.entityTypeAttribute)
                            && a.MapToType<EntityTypeAttribute>().Abstract == false))
                {
                    var wrapper = new EntityTypeWrapper(typeSymbol);
                    wrapper.InitWithErrorHandling(this);
                    yield return wrapper;
                }
            }
        }
    }
}