using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Meta
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

        public async Task<List<StaticClassSymbolWrapper>> GetStaticClassesWithExportedMethods()
        {
            var exported = await SymbolFinder.FindReferencesAsync(
                RelevantSymbols.Instance.exportAttribute, _solution);
            var classes = new HashSet<INamedTypeSymbol>();
            var result = new List<StaticClassSymbolWrapper>();

            foreach (var s in exported)
            {
                if (s.Definition is IMethodSymbol method 
                    && method.ContainingType.IsStatic
                    && !classes.Contains(method.ContainingType))
                {
                    var classWrapper = new StaticClassSymbolWrapper(method.ContainingType);
                    classWrapper.Init(this);
                    result.Add(classWrapper);

                    classes.Add(method.ContainingType);
                }
            }

            return result;
        }
    }
}