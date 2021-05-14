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
        public GenerationEnvironment env;

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

            env = new GenerationEnvironment(projectPaths);

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
            await env.Reset(project);

            // Find function
            // Potential context dependency
            // Initialization function, after initialization function
            // File name
            // Printer initialization
            // Writing

            // TODO: parallelize
            var behaviorWrappers = (await env.FindAllBehaviors())
                .Select(b => new BehaviorSymbolWrapper(b))
                .InitAndAfterInit(env)
                .ToArray();
            
            var componentWrappers = (await env.FindAllDirectComponents())
                .Select(c => new ComponentSymbolWrapper(c))
                .InitAndAfterInit(env)
                .ToArray();

            var tagWrappers = (await env.FindAllTags())
                .Select(t => new TagSymbolWrapper(t))
                .InitAndAfterInit(env)
                .ToArray();
        
            foreach (var behavior in behaviorWrappers)
            {
                behavior.WriteGenerationMessage();

                (new BehaviorPrinter(behavior))
                    .WriteToFile($"{env.Paths.BehaviorAutogenFolder}/{behavior.ClassName}.cs");
            }

            foreach (var component in componentWrappers)
            {
                component.WriteGenerationMessage();

                (new ComponentPrinter(component))
                    .WriteToFile($"{env.Paths.ComponentAutogenFolder}/{component.ClassName}.cs");
            }

            foreach (var tag in tagWrappers)
            {
                tag.WriteGenerationMessage();

                (new ComponentPrinter(tag)).WriteToFile(
                    $"{env.Paths.TagsAutogenFolder}/{tag.ClassName}.cs");
            }

            var methodClasses = env.GetExportedMethodClasses();
            {
                foreach (var methodClass in methodClasses)
                {
                    methodClass.WriteGenerationMessage();

                    (new ChainHandlersPrinter(methodClass))
                        .WriteToFile($"{env.Paths.HandlersAutogenFolder}/{methodClass.ClassName}.cs");
                }
            }

            
            var topLevelStatTypes = GetJsonFileNames(env.Paths.StatJsonsFolder)
                .Select(fname => StatType.ParseJson(env.statParsingContext, fname))
                .WhereNotNull();
            {
                var startPrinter = new StatStartPrinter(env.RootNamespaceName);

                foreach (var stat in topLevelStatTypes)
                {
                    Console.WriteLine($"Generating code for stat {stat.Name}");

                    startPrinter.ResetStat(stat);
                    startPrinter.WriteToFile($@"{env.Paths.StatAutogenFolder}/{stat.Name}.cs");
                }
            }

            var slots = env.GetSlots();

            {
                (new SlotExtensionsPrinter(env.RootNamespaceName, slots))
                    .WriteToFile(env.Paths.SlotExtensionsPath);
            }

            var flagEnums = env.GetFlagEnums()
                .Select(f => new FlagEnumSymbolWrapper(f))
                .Where(f => f.TryInit(env));

            foreach (var flag in flagEnums)
            {
                (new FlagsPrinter(flag))
                    .WriteToFile($"{env.Paths.FlagsAutogenFolder}/{flag.ClassName}.cs");
            }

            var entityTypes = env.GetEntityTypes();
            var methodClassInstances = env.GetMethodClassInstances();
            var fieldsRequiringInit = env.GetFieldsRequiringInit();
            var staticIndentiyingStatFields = env.GetStaticIdentifyingStatFields();

            {
                // They must live in at least the base namespace hopper
                if (env.RootNamespaceName.Length >= "Hopper".Length)
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
                        statRootScope = env.statParsingContext.currentScope,
                        slots = slots,
                        Namespace = env.RootNamespaceName
                    };

                    Console.WriteLine("Generating code for the main init function");

                    File.WriteAllText(env.Paths.MainAutogenFile, mainPrinter.TransformText(), Encoding.UTF8);
                }
                else
                {
                    Console.WriteLine($"Unable to generate code for the main init function: The common namespace between the files that participate in code generation must at least contain 'Hopper' (got '{env.RootNamespaceName})'");
                }

            }
            // var components = await context.FindAllDirectComponents();
            // var tags = await context.FindAllTags();
        }
    }

}