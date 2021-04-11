using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hopper.Meta.Template;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace Meta
{
    public class Generator
    {
        const string coreProjectPath = @"../Core/Hopper_Core.csproj";
        const string sharedProjectPath = @"../Shared/Hopper_Shared.csproj";
        const string autogenFolder = @"../Core/Autogen";
        static readonly string behaviorAutogenFolder = $@"{autogenFolder}/Behaviors";
        static readonly string componentAutogenFolder = $@"{autogenFolder}/Components";
        static readonly string handlersAutogenFolder = $@"{autogenFolder}/Handlers";

        public MSBuildWorkspace msWorkspace;
        public Project coreProject;
        public bool failFlag = false;

        public async Task<bool> Start(params string[] projectPaths)
        {
            await InitWorkspace(projectPaths);
            if (!failFlag)
            {
                await Generate();
            }
            return failFlag;
        } 

        public async Task<MSBuildWorkspace> InitWorkspace(string[] projectPaths)
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

            msWorkspace.WorkspaceFailed += (s, args) => 
            {
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

            if (!Directory.Exists(autogenFolder))
                Directory.CreateDirectory(autogenFolder);

            CreateOrEmptyDirectory(behaviorAutogenFolder);
            CreateOrEmptyDirectory(componentAutogenFolder);
            CreateOrEmptyDirectory(handlersAutogenFolder);

            await msWorkspace.OpenProjectAsync(sharedProjectPath);

            coreProject = await msWorkspace.OpenProjectAsync(coreProjectPath);
            
            // Open the mod projects
            foreach (var projectName in projectPaths)
                await msWorkspace.OpenProjectAsync(projectName);

            return msWorkspace;
        }

        public static void CreateOrEmptyDirectory(string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            else
            foreach (var file in new DirectoryInfo(directory).GetFiles())
                file.Delete(); 
        }

        public async Task Generate()
        {
            var ctx = new ProjectContext(msWorkspace.CurrentSolution);
            await ctx.Reset(coreProject);

            // TODO: parallelize
            {
                var behaviors = await ctx.FindAllBehaviors();
                var behaviorWrappers = new List<BehaviorSymbolWrapper>();
                
                foreach (var behavior in behaviors)
                {
                    try
                    {
                        var wrapped = new BehaviorSymbolWrapper(behavior);
                        wrapped.Init(ctx);
                        behaviorWrappers.Add(wrapped);
                    }
                    catch (GeneratorException e)
                    {
                        Console.WriteLine("An error occured while processing a behavior:");
                        Console.WriteLine(e.Message);
                        Console.WriteLine("");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                // After init must be called after all of the behaviors have been added to the dictionary
                foreach (var behavior in behaviorWrappers)
                {
                    try
                    {
                        behavior.AfterInit(ctx);   
                    }
                    catch (GeneratorException e)
                    {
                        Console.WriteLine("An error occured while processing a behavior:");
                        Console.WriteLine(e.Message);
                        Console.WriteLine("");
                    } 
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }               
                }
                foreach (var behavior in behaviorWrappers)
                {
                    var behaviorPrinter = new BehaviorCode();
                    behaviorPrinter.Initialize();
                    behaviorPrinter.behavior = behavior;

                    var behaviorDocument = ctx._solution.GetDocument(behavior.symbol.Locations.First().SourceTree);

                    Console.WriteLine($"Generating code for {behavior.Calling}");

                    File.WriteAllText(
                        $"{behaviorAutogenFolder}/{behavior.ClassName}.cs",
                        behaviorPrinter.TransformText(),
                        Encoding.UTF8);
                }
            }
            {
                var components = await ctx.FindAllDirectComponents();
                var componentWrappers = new List<ComponentSymbolWrapper>();
                
                foreach (var b in components)
                {
                    try
                    {
                        var wrapped = new ComponentSymbolWrapper(b);
                        wrapped.Init(ctx);
                        componentWrappers.Add(wrapped);
                    }
                    catch (GeneratorException e)
                    {
                        Console.WriteLine("An error occured while processing a component:");
                        Console.WriteLine(e.Message);
                        Console.WriteLine("");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                foreach (var component in componentWrappers)
                {
                    var componentPrinter = new ComponentCode();
                    componentPrinter.Initialize();
                    componentPrinter.component = component;

                    Console.WriteLine($"Generating code for {component.Calling}");

                    File.WriteAllText(
                        $"{componentAutogenFolder}/{component.ClassName}.cs",
                        componentPrinter.TransformText(),
                        Encoding.UTF8);
                }
            }

            {
                var staticClassesWithExportedMethods = ctx.GetStaticClassesWithExportedMethods();
                foreach (var staticClass in staticClassesWithExportedMethods)
                {
                    var handlersPrinter = new ChainHandlersCode();
                    handlersPrinter.staticClass = staticClass;

                    Console.WriteLine($"Generating code for {staticClass.Calling}");

                    File.WriteAllText(
                        $"{handlersAutogenFolder}/{staticClass.ClassName}.cs",
                        handlersPrinter.TransformText(),
                        Encoding.UTF8);
                }
            }
            // var components = await ctx.FindAllDirectComponents();
            // var tags = await ctx.FindAllTags();
        }

        public async Task Test2()
        {
            var ctx = new ProjectContext(msWorkspace.CurrentSolution);
            await ctx.Reset(coreProject);
            
            var implementations = await ctx.FindAllDirectComponents();

            Console.WriteLine(implementations.Count());

            foreach (var behavior in implementations)
            {
                foreach (var method in behavior.GetMembers().OfType<IMethodSymbol>())
                foreach (var attrib in method.GetAttributes())
                {
                    if (SymbolEqualityComparer.Default.Equals(attrib.AttributeClass, RelevantSymbols.Instance.aliasAttribute))
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