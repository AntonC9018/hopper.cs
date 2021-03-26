using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using Hopper.Meta.Template;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Meta
{
    class Program
    {
        public static async Task Main()
        {
            MSBuildLocator.RegisterDefaults();
            msWorkspace = await InitWorkspace();
            if (!failFlag)
            {
                await Test2();
            }
        }

        static bool failFlag = false;
        static MSBuildWorkspace msWorkspace;
        const string coreProjectPath = @"../Core/Hopper_Core.csproj";
        static Project coreProject;

        public static async Task<MSBuildWorkspace> InitWorkspace(params string[] projectPaths)
        {
            try
            {
                msWorkspace = MSBuildWorkspace.Create();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                return null;
            }

            msWorkspace.WorkspaceFailed += (s, args) => {
                if (args.Diagnostic.Kind == WorkspaceDiagnosticKind.Failure)
                {
                    Console.WriteLine($"Unable to open the project.\n {args.Diagnostic.Message}");
                    failFlag = true;
                }
                else
                {
                    Console.WriteLine($"Warning while opening a project:\n {args.Diagnostic.Message}");
                }
            };

            coreProject = await msWorkspace.OpenProjectAsync(coreProjectPath);
            
            // Open the core project or the mod
            foreach (var projectName in projectPaths)
                await msWorkspace.OpenProjectAsync(projectName);

            return msWorkspace;
        }


        public class RelevantSymbols
        {
            public INamedTypeSymbol icomponent;
            public INamedTypeSymbol ibehavior;
            public INamedTypeSymbol itag;
            public INamedTypeSymbol aliasAttribute;
            public INamedTypeSymbol autoActivationAttribute;
            public INamedTypeSymbol chainsAttribute;
            public INamedTypeSymbol injectAttribute;
            public INamedTypeSymbol flagsAttribute;
            public INamedTypeSymbol exportAttribute;
            
            public static INamedTypeSymbol GetComponentSymbol(Compilation compilation, string name)
            {
                return (INamedTypeSymbol)compilation.GetTypeByMetadataName($"Hopper.Core.Components.{name}");
            }

            public void Init(Compilation compilation)
            {
                icomponent      = GetComponentSymbol(compilation, "IComponent");
                ibehavior       = GetComponentSymbol(compilation, "IBehavior");
                itag            = GetComponentSymbol(compilation, "IBehavior");
                aliasAttribute  = GetComponentSymbol(compilation, "AliasAttribute");
                chainsAttribute = GetComponentSymbol(compilation, "ChainsAttribute");
                injectAttribute = GetComponentSymbol(compilation, "InjectAttribute");
                flagsAttribute  = GetComponentSymbol(compilation, "FlagsAttribute");
                exportAttribute = GetComponentSymbol(compilation, "ExportAttribute");
                autoActivationAttribute = GetComponentSymbol(compilation, "AutoActivationAttribute");
            }
        }

        public class ProjectContext
        {
            public Solution solution;
            public Project project;

            public HashSet<Project> projectSet;
            public Compilation compilation;
            public RelevantSymbols relevantSymbols;

            public async void Init()
            {
                projectSet = new HashSet<Project>{project};
                compilation = await project.GetCompilationAsync();
            }

            public Task<IEnumerable<INamedTypeSymbol>> FindAllComponents()
            {
                return SymbolFinder.FindImplementationsAsync(
                    relevantSymbols.icomponent, solution, transitive: true, projectSet.ToImmutableHashSet()
                );
            }

            public Task<IEnumerable<INamedTypeSymbol>> FindAllBehaviors()
            {
                return SymbolFinder.FindImplementationsAsync(
                    relevantSymbols.ibehavior, solution, transitive: true, projectSet.ToImmutableHashSet()
                );
            }
        }

        public class MethodSymbolWrapper
        {
            public IMethodSymbol symbol;
            public string alias = null;

            public void Adapter(RelevantSymbols relevantSymbols)
            {
                if (!symbol.IsStatic)
                {
                    // do               ctx.actor.Get<Name>().<MethodName>(whatever)
                    // otherwise, do    <Name>.<MethodName>(whatever)
                }

                foreach (var s in symbol.Parameters)
                {
                    if (/* ctx contains that name directly */true)
                    {
                        // if (s.RefKind == RefKind.Out)
                        Console.WriteLine($"var {s.Name} = ctx.{s.Name}");
                    }
                    else if (s.ContainingType.Interfaces.Contains(relevantSymbols.icomponent))
                    {
                        // TODO: if the name contains the name of an entity type field
                        //       of the context followed by an underscore, get the component
                        //       from that entity and save it.
                        // TODO: get the component from entity. For now, assume that
                        //       the entity is assumed to always contain the given component.
                        Console.WriteLine($"var {s.Name} = ctx.entity.Get{s.ContainingType.Name}();");
                    }
                    else
                    {
                        // TODO: report an error
                    }
                    Console.WriteLine($"{s.Name} of type {s.ContainingType.Name}");
                }
            }

        }


        public class ComponentSymbolWrapper
        {   
            public ProjectContext context;
            public INamedTypeSymbol symbol;
            public HashSet<IFieldSymbol> flaggedFields;
            public HashSet<string> aliases;

            public bool IsBehavior => symbol.Interfaces.Contains(context.relevantSymbols.ibehavior);
            public bool IsTag => symbol.Interfaces.Contains(context.relevantSymbols.itag);
        }

        public class BehaviorSymbolWrapper : ComponentSymbolWrapper
        {

        }

        public static async Task Test2()
        {
            var ctx = new ProjectContext();
            ctx.project = coreProject;
            ctx.solution = msWorkspace.CurrentSolution;
            
            var implementations = await ctx.FindAllComponents();

            Console.WriteLine(implementations.Count());

            foreach (var behavior in implementations)
            {
                foreach (var method in behavior.GetMembers().OfType<IMethodSymbol>())
                foreach (var attrib in method.GetAttributes())
                {
                    if (SymbolEqualityComparer.Default.Equals(attrib.AttributeClass, aliasAttribute))
                    foreach (var arg in attrib.ConstructorArguments)
                    {
                        var t = (TypedConstant)arg;
                        Console.WriteLine(t.Value);
                    }
                }
            }
            return;
        }
    }
}