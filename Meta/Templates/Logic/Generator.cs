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
        const string sharedProjectPath = @"../Shared/Hopper_Shared.csproj";

        public MSBuildWorkspace msWorkspace;
        public ProjectContext context;

        public Generator() 
        {
        }

        public async Task Start(params string[] projectPaths)
        {
            if (await InitWorkspace(projectPaths))
            {
                foreach (var projectPath in projectPaths)
                {
                    Console.WriteLine($"Appending {projectPath}");
                    var project = await msWorkspace.OpenProjectAsync(projectPath);
                    await Generate(project);
                }
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

            context = new ProjectContext(projectPaths);

            await msWorkspace.OpenProjectAsync(sharedProjectPath);

            return success;
        }

        public static IEnumerable<string> GetJsonFileNames(string directory)
        {
            foreach (var file in Directory.EnumerateFiles(directory))
            {
                yield return file;
            }
        }

        public async Task Generate(Project project)
        {
            await context.Reset(project);

            // TODO: parallelize
            var behaviorWrappers = new List<BehaviorSymbolWrapper>();
            {
                foreach (var behavior in await context.FindAllBehaviors())
                {
                    var wrapped = new BehaviorSymbolWrapper(behavior);
                    if (wrapped.InitWithErrorHandling(context))
                        behaviorWrappers.Add(wrapped);
                }
                // After init must be called after all of the behaviors have been added to the dictionary
                foreach (var behavior in behaviorWrappers)
                {
                    behavior.AfterInitWithErrorHandling(context);              
                }
                foreach (var behavior in behaviorWrappers)
                {
                    var behaviorPrinter = new BehaviorCode();
                    behaviorPrinter.behavior = behavior;

                    Console.WriteLine($"Generating code for {behavior.Calling}");

                    File.WriteAllText(
                        $"{context._paths.BehaviorAutogenFolder}/{behavior.ClassName}.cs",
                        behaviorPrinter.TransformText(),
                        Encoding.UTF8);
                }
            }

            var componentWrappers = new List<ComponentSymbolWrapper>();
            {
                var components = await context.FindAllDirectComponents();
                
                foreach (var b in components)
                {
                    var wrapped = new ComponentSymbolWrapper(b);
                    if (wrapped.InitWithErrorHandling(context))
                        componentWrappers.Add(wrapped);
                }
                foreach (var component in componentWrappers)
                {
                    component.AfterInitWithErrorHandling(context);               
                }
                foreach (var component in componentWrappers)
                {
                    var componentPrinter = new ComponentCode();
                    componentPrinter.component = component;

                    Console.WriteLine($"Generating code for {component.Calling}");

                    File.WriteAllText(
                        $"{context._paths.ComponentAutogenFolder}/{component.ClassName}.cs",
                        componentPrinter.TransformText(),
                        Encoding.UTF8);
                }
            }

            var tagWrappers = new List<ComponentSymbolWrapper>();
            {
                var tags = await context.FindAllTags();
                
                foreach (var b in tags)
                {
                    var wrapped = new TagSymbolWrapper(b);
                    if (wrapped.InitWithErrorHandling(context))
                        tagWrappers.Add(wrapped);
                }
                foreach (var component in tagWrappers)
                {
                    component.AfterInitWithErrorHandling(context);               
                }
                foreach (var component in tagWrappers)
                {
                    var componentPrinter = new ComponentCode();
                    componentPrinter.component = component;

                    Console.WriteLine($"Generating code for {component.Calling}");

                    File.WriteAllText(
                        $"{context._paths.TagsAutogenFolder}/{component.ClassName}.cs",
                        componentPrinter.TransformText(),
                        Encoding.UTF8);
                }
            }

            var methodClasses = context.GetExportedMethodClasses();
            {
                foreach (var methodClass in methodClasses)
                {
                    var handlersPrinter = new ChainHandlersCode();
                    handlersPrinter.methodClass = methodClass;

                    Console.WriteLine($"Generating code for {methodClass.Calling}");

                    File.WriteAllText(
                        $"{context._paths.HandlersAutogenFolder}/{methodClass.ClassName}.cs",
                        handlersPrinter.TransformText(),
                        Encoding.UTF8);
                }
            }

            
            var topLevelStatTypes = GetJsonFileNames(context._paths.StatJsonsFolder).Select(
                fname => StatType.ParseJson(context.statParsingContext, fname));
            {
                var startPrinter = new StatStartCode();
                var subPrinter = new StatCode();
                startPrinter.statCodePrinter = subPrinter;
                startPrinter.Namespace = $"{context.RootNamespaceName}.Stat";

                foreach (var stat in topLevelStatTypes)
                {
                    Console.WriteLine($"Generating code for stat {stat.Name}");

                    subPrinter.stat = stat;

                    File.WriteAllText(
                        $@"{context._paths.StatAutogenFolder}/{stat.Name}.cs",
                        startPrinter.TransformText(),
                        Encoding.UTF8);
                }
            }

            var entityTypes = context.GetEntityTypes();
            var slots = context.GetSlots();
            var methodClassInstances = context.GetMethodClassInstances();
            var fieldsRequiringInit = context.GetFieldsRequiringInit();
            var staticIndentiyingStatFields = context.GetStaticIdentifyingStatFields();

            {
                // They must live in at least the base namespace hopper
                if (context.RootNamespaceName.Length >= "Hopper".Length)
                {
                    var mainPrinter = new AllInitCode()
                    {
                        components = componentWrappers,
                        behaviors = behaviorWrappers,
                        methodClasses = methodClasses,
                        methodClassInstances = methodClassInstances,
                        fieldsRequiringInit = fieldsRequiringInit,
                        staticIndentiyingStatFields = staticIndentiyingStatFields,
                        entityTypes = entityTypes,
                        statRootScope = context.statParsingContext.currentScope,
                        slots = slots,
                        Namespace = context.RootNamespaceName
                    };

                    Console.WriteLine("Generating code for the main init function");

                    File.WriteAllText(context._paths.MainAutogenFile, mainPrinter.TransformText(), Encoding.UTF8);
                }
                else
                {
                    Console.WriteLine($"The common namespace between components must at least contain 'Hopper' (got '{context.RootNamespaceName})'");
                }

            }
            // var components = await context.FindAllDirectComponents();
            // var tags = await context.FindAllTags();
        }
    }

}