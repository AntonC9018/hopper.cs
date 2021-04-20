using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hopper.Meta.Stats;
using Hopper.Meta.Template;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace Hopper.Meta
{
    public class Generator
    {
        const string coreProjectPath = @"../Core/Hopper_Core.csproj";
        const string sharedProjectPath = @"../Shared/Hopper_Shared.csproj";
        const string autogenFolder = @"../Core/Autogen";
        const string statJsonsFolder = @"../Core/Stats/Json";
        static readonly string behaviorAutogenFolder = $@"{autogenFolder}/Behaviors";
        static readonly string componentAutogenFolder = $@"{autogenFolder}/Components";
        static readonly string tagsAutogenFolder = $@"{autogenFolder}/Tags";
        static readonly string handlersAutogenFolder = $@"{autogenFolder}/Handlers";
        static readonly string mainAutogenFile = $@"{autogenFolder}/Main.cs";
        static readonly string statAutogenFolder = $@"{autogenFolder}/Stats";

        public MSBuildWorkspace msWorkspace;
        public Project coreProject;

        public async Task Start(params string[] projectPaths)
        {
            if (await InitWorkspace(projectPaths))
            {
                await Generate();
            }
        } 

        public async Task<bool> InitWorkspace(string[] projectPaths)
        {
            try
            {
                msWorkspace = MSBuildWorkspace.Create();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                return false;
            }

            bool success = true;

            msWorkspace.WorkspaceFailed += (s, args) => 
            {
                if (args.Diagnostic.Kind == WorkspaceDiagnosticKind.Failure)
                {
                    Console.WriteLine($"Unable to open the project.\n {args.Diagnostic.Message}");
                    success = false;
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
            CreateOrEmptyDirectory(tagsAutogenFolder);
            CreateOrEmptyDirectory(statAutogenFolder);

            await msWorkspace.OpenProjectAsync(sharedProjectPath);

            coreProject = await msWorkspace.OpenProjectAsync(coreProjectPath);
            
            // Open the mod projects
            foreach (var projectName in projectPaths)
                await msWorkspace.OpenProjectAsync(projectName);

            return success;
        }

        public static void CreateOrEmptyDirectory(string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            else
            foreach (var file in new DirectoryInfo(directory).GetFiles())
                file.Delete(); 
        }

        public static IEnumerable<string> GetJsonFileNames(string directory)
        {
            foreach (var file in Directory.EnumerateFiles(directory))
            {
                yield return file;
            }
        }

        public async Task Generate()
        {
            var ctx = new ProjectContext(msWorkspace.CurrentSolution);
            await ctx.Reset(coreProject);

            // TODO: parallelize
            var behaviorWrappers = new List<BehaviorSymbolWrapper>();
            {
                foreach (var behavior in await ctx.FindAllBehaviors())
                {
                    var wrapped = new BehaviorSymbolWrapper(behavior);
                    if (wrapped.InitWithErrorHandling(ctx))
                        behaviorWrappers.Add(wrapped);
                }
                // After init must be called after all of the behaviors have been added to the dictionary
                foreach (var behavior in behaviorWrappers)
                {
                    behavior.AfterInitWithErrorHandling(ctx);              
                }
                foreach (var behavior in behaviorWrappers)
                {
                    var behaviorPrinter = new BehaviorCode();
                    behaviorPrinter.behavior = behavior;

                    Console.WriteLine($"Generating code for {behavior.Calling}");

                    File.WriteAllText(
                        $"{behaviorAutogenFolder}/{behavior.ClassName}.cs",
                        behaviorPrinter.TransformText(),
                        Encoding.UTF8);
                }
            }

            var componentWrappers = new List<ComponentSymbolWrapper>();
            {
                var components = await ctx.FindAllDirectComponents();
                
                foreach (var b in components)
                {
                    var wrapped = new ComponentSymbolWrapper(b);
                    if (wrapped.InitWithErrorHandling(ctx))
                        componentWrappers.Add(wrapped);
                }
                foreach (var component in componentWrappers)
                {
                    component.AfterInitWithErrorHandling(ctx);               
                }
                foreach (var component in componentWrappers)
                {
                    var componentPrinter = new ComponentCode();
                    componentPrinter.component = component;

                    Console.WriteLine($"Generating code for {component.Calling}");

                    File.WriteAllText(
                        $"{componentAutogenFolder}/{component.ClassName}.cs",
                        componentPrinter.TransformText(),
                        Encoding.UTF8);
                }
            }

            var tagWrappers = new List<ComponentSymbolWrapper>();
            {
                var tags = await ctx.FindAllTags();
                
                foreach (var b in tags)
                {
                    var wrapped = new TagSymbolWrapper(b);
                    if (wrapped.InitWithErrorHandling(ctx))
                        tagWrappers.Add(wrapped);
                }
                foreach (var component in tagWrappers)
                {
                    component.AfterInitWithErrorHandling(ctx);               
                }
                foreach (var component in tagWrappers)
                {
                    var componentPrinter = new ComponentCode();
                    componentPrinter.component = component;

                    Console.WriteLine($"Generating code for {component.Calling}");

                    File.WriteAllText(
                        $"{tagsAutogenFolder}/{component.ClassName}.cs",
                        componentPrinter.TransformText(),
                        Encoding.UTF8);
                }
            }

            var staticClassesWithExportedMethods = ctx.GetStaticClassesWithExportedMethods();
            {
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

            // TODO: do this before any generation has occured
            string commonNamespace;
            {
                string reference = behaviorWrappers[0].Namespace;
                int commonPartEndIndex = reference.Length;

                foreach (var p in componentWrappers)
                {
                    int index = p.Namespace.IndexOfFirstDifference(reference);
                    if (index != -1 && index < commonPartEndIndex)
                    {
                        commonPartEndIndex = index;
                    }
                }

                commonNamespace = reference.Substring(0, commonPartEndIndex);
            }

            var statNamespace = $"{commonNamespace}.Stat.Basic";
            var statContext = new ParsingContext(statNamespace);
            var topLevelStatTypes = GetJsonFileNames(statJsonsFolder).Select(
                fname => StatType.ParseJson(statContext, fname));
            {
                var startPrinter = new StatStartCode();
                var subPrinter = new StatCode();
                startPrinter.statCodePrinter = subPrinter;
                startPrinter.Namespace = statNamespace;

                foreach (var stat in topLevelStatTypes)
                {
                    Console.WriteLine($"Generating code for stat {stat.Name}");

                    subPrinter.stat = stat;

                    File.WriteAllText(
                        $@"{statAutogenFolder}/{stat.Name}.cs",
                        startPrinter.TransformText(),
                        Encoding.UTF8);
                }
            }

            var entityTypes = ctx.GetEntityTypes();
            {
                // They must live in at least the base namespace hopper
                if (commonNamespace.Length >= "Hopper".Length)
                {
                    var mainPrinter = new AllInitCode()
                    {
                        components = componentWrappers,
                        behaviors = behaviorWrappers,
                        staticClasses = staticClassesWithExportedMethods,
                        entityTypes = entityTypes,
                        statRootScope = statContext.scope,
                        Namespace = commonNamespace
                    };

                    Console.WriteLine("Generating code for the main init function");

                    File.WriteAllText(mainAutogenFile, mainPrinter.TransformText(), Encoding.UTF8);
                }
                else
                {
                    Console.WriteLine($"The common namespace between components must at least contain 'Hopper' (got '{commonNamespace})'");
                }

            }
            // var components = await ctx.FindAllDirectComponents();
            // var tags = await ctx.FindAllTags();
        }

        public async Task Test2()
        {
            var ctx = new ProjectContext(msWorkspace.CurrentSolution);
            await ctx.Reset(coreProject);
            
            var implementations = await ctx.FindAllTags();

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