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
        const string sharedProjectPath = @"../Shared/Hopper.Shared.csproj";

        public MSBuildWorkspace msWorkspace;
        public GenerationEnvironment context;

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

            context = new GenerationEnvironment(projectPaths);

            await msWorkspace.OpenProjectAsync(sharedProjectPath);

            return success;
        }

        public static IEnumerable<string> GetJsonFileNames(string directory)
        {
            foreach (var file in Directory.EnumerateFiles(directory))
            {
                if (Path.GetExtension(file).ToLower() == ".json")
                    yield return file;
            }
        }

        public async Task Generate(Project project)
        {
            await context.Reset(project);

            // Find function
            // Potential context dependency
            // Initialization function, after initialization function
            // File name
            // Printer initialization
            // Writing

            // TODO: parallelize
            var behaviorWrappers = (await context.FindAllBehaviors())
                .Select(b => new BehaviorSymbolWrapper(b))
                .InitAndAfterInit(context);
            
            var componentWrappers = (await context.FindAllDirectComponents())
                .Select(c => new ComponentSymbolWrapper(c))
                .InitAndAfterInit(context);

            var tagWrappers = (await context.FindAllTags())
                .Select(t => new TagSymbolWrapper(t))
                .InitAndAfterInit(context);
        
            foreach (var behavior in behaviorWrappers)
            {
                behavior.WriteGenerationMessage();

                (new BehaviorPrinter(behavior))
                    .WriteToFile($"{context._paths.BehaviorAutogenFolder}/{behavior.ClassName}.cs");
            }

            foreach (var component in componentWrappers)
            {
                component.WriteGenerationMessage();

                (new ComponentPrinter(component))
                    .WriteToFile($"{context._paths.ComponentAutogenFolder}/{component.ClassName}.cs");
            }

            foreach (var tag in tagWrappers)
            {
                tag.WriteGenerationMessage();

                (new ComponentPrinter(tag)).WriteToFile(
                    $"{context._paths.TagsAutogenFolder}/{tag.ClassName}.cs");
            }

            var methodClasses = context.GetExportedMethodClasses();
            {
                foreach (var methodClass in methodClasses)
                {
                    methodClass.WriteGenerationMessage();

                    (new ChainHandlersPrinter(methodClass))
                        .WriteToFile($"{context._paths.HandlersAutogenFolder}/{methodClass.ClassName}.cs");
                }
            }

            
            var topLevelStatTypes = GetJsonFileNames(context._paths.StatJsonsFolder).Select(
                fname => StatType.ParseJson(context.statParsingContext, fname));
            {
                var startPrinter = new StatStartPrinter(context.RootNamespaceName);

                foreach (var stat in topLevelStatTypes)
                {
                    Console.WriteLine($"Generating code for stat {stat.Name}");

                    startPrinter.ResetStat(stat);
                    startPrinter.WriteToFile($@"{context._paths.StatAutogenFolder}/{stat.Name}.cs");
                }
            }

            var slots = context.GetSlots();

            {
                (new SlotExtensionsPrinter(context.RootNamespaceName, slots))
                    .WriteToFile(context._paths.SlotExtensionsPath);
            }

            var flagEnums = context.GetFlagEnums()
                .Select(f => new FlagEnumSymbolWrapper(f))
                .Where(f => f.InitWithErrorHandling(context));

            foreach (var flag in flagEnums)
            {
                (new FlagsPrinter(flag))
                    .WriteToFile($"{context._paths.FlagsAutogenFolder}/{flag.ClassName}.cs");
            }

            var entityTypes = context.GetEntityTypes();
            var methodClassInstances = context.GetMethodClassInstances();
            var fieldsRequiringInit = context.GetFieldsRequiringInit();
            var staticIndentiyingStatFields = context.GetStaticIdentifyingStatFields();

            {
                // They must live in at least the base namespace hopper
                if (context.RootNamespaceName.Length >= "Hopper".Length)
                {
                    var mainPrinter = new AllInitPrinter()
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
                    Console.WriteLine($"Unable to generate code for the main init function: The common namespace between the files that participate in code generation must at least contain 'Hopper' (got '{context.RootNamespaceName})'");
                }

            }
            // var components = await context.FindAllDirectComponents();
            // var tags = await context.FindAllTags();
        }
    }

}