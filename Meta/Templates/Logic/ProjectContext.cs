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

        public ProjectContext(Solution solution)
        {
            _solution = solution;
            globalAliases = new HashSet<string>();
        }

        public Project _project;

        public HashSet<Project> projectSet;
        public Compilation compilation;

        public HashSet<string> globalAliases;

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
    }
}