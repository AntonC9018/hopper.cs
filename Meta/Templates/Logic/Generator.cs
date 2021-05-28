using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class ModProject
    {
        public string AssemblyName;
        public string ProjectPath;
        public bool ToGenerate => ProjectPath != null;

        public ModProject(string assemblyName, string projectPath = null)
        {
            AssemblyName = assemblyName;
            ProjectPath = projectPath;
        }
    }

    public class Generator
    {
        const string sharedProjectPath = @"../Shared/Hopper.Shared.csproj";

        public MSBuildWorkspace msWorkspace;
        public bool projectLoadFailure;
        public GenerationEnvironment _env;

        public Generator() 
        {
        }

        public async Task Start(IEnumerable<ModProject> projects)
        {
            // TODO: do error checking here. Make sure the last thing references all previous ones.
            var last = projects.Last();

            // Make sure the last one generates code, otherwise it's not needed in the list
            Debug.Assert(last.ToGenerate, "The last project in list must generate code, otherwise it's pointless");

            // Make sure at least one of the projects generates code
            // Debug.Assert(projects.Any(t => t.ToGenerate), "Nothing generates code");
        
            if (!InitWorkspace()) return;

            _env = new GenerationEnvironment(projects);
            var lastProject = await msWorkspace.OpenProjectAsync(last.ProjectPath); 

            if (projectLoadFailure) return;

            _env.Init(lastProject.Solution, await lastProject.GetCompilationAsync());

            // Now, go through the referenced projects, specified as projects manually,
            // and analyze their code, perhaps generate is we need to generate code 
            foreach (var project in projects)
            {
                Console.WriteLine($"Appending {project.AssemblyName}");
                await Generate(project);
            }
        } 

        public bool InitWorkspace()
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

            projectLoadFailure = false;

            msWorkspace.WorkspaceFailed += (s, args) => 
            {
                if (args.Diagnostic.Kind == WorkspaceDiagnosticKind.Failure)
                {
                    Console.WriteLine($"Unable to open the project.\n {args.Diagnostic.Message}");
                    projectLoadFailure = true;
                }
                else
                {
                    Console.WriteLine($"Warning while opening a project:\n {args.Diagnostic.Message}");
                }
            };

            return true;
        }

        public static IEnumerable<string> GetJsonFileNames(string directory)
        {
            if (Directory.Exists(directory))
            {
                foreach (var file in Directory.EnumerateFiles(directory))
                {
                    if (Path.GetExtension(file).ToLower() == ".json")
                        yield return file;
                }
            }
        }

        public async Task Generate(ModProject project)
        {
            _env.ResetProject(project);

            // Find function
            // Potential context dependency
            // Initialization function, after initialization function
            // File name
            // Printer initialization
            // Writing

            // TODO: parallelize
            var behaviorWrappers = (await _env.FindAllBehaviors())
                .Select(b => new BehaviorSymbolWrapper(b))
                .InitAndAfterInit(_env)
                .ToArray();
            
            var componentWrappers = (await _env.FindAllDirectComponents())
                .Select(c => new ComponentSymbolWrapper(c))
                .InitAndAfterInit(_env)
                .ToArray();

            var tagWrappers = (await _env.FindAllTags())
                .Select(t => new TagSymbolWrapper(t))
                .InitAndAfterInit(_env)
                .ToArray();

            var exportingClasses = _env.GetOtherExportingClasses().ToArray();

            var topLevelStatTypes = GetJsonFileNames(_env.Paths.StatJsonsFolder)
                .Select(fname => StatType.ParseJson(_env.statParsingContext, fname))
                .WhereNotNull()
                .ToArray();

            var slots = _env.GetSlots().ToArray();

            var flagEnums = _env.GetFlagEnums()
                .Select(f => new FlagEnumSymbolWrapper(f))
                .Where(f => f.TryInit(_env))
                .ToArray();
            
            var entityTypes = _env.GetEntityTypes().ToArray();
            var methodClassInstances = _env.GetMethodClassInstances().ToArray();
            var fieldsRequiringInit = _env.GetFieldsRequiringInit().ToArray();
            var staticIndentiyingStatFields = _env.GetStaticIdentifyingStatFields().ToArray();

            if (!project.ToGenerate) return;

            foreach (var behavior in behaviorWrappers)
            {
                behavior.WriteGenerationMessage();

                (new BehaviorPrinter(behavior))
                    .WriteToFile($"{_env.Paths.BehaviorAutogenFolder}/{behavior.ClassName}.cs");
            }

            foreach (var component in componentWrappers)
            {
                component.WriteGenerationMessage();

                (new ComponentPrinter(component))
                    .WriteToFile($"{_env.Paths.ComponentAutogenFolder}/{component.ClassName}.cs");
            }

            foreach (var tag in tagWrappers)
            {
                tag.WriteGenerationMessage();

                (new ComponentPrinter(tag)).WriteToFile(
                    $"{_env.Paths.TagsAutogenFolder}/{tag.ClassName}.cs");
            }

            {
                foreach (var methodClass in exportingClasses)
                {
                    methodClass.WriteGenerationMessage();

                    (new ExportedStuffPrinter(methodClass))
                        .WriteToFile($"{_env.Paths.HandlersAutogenFolder}/{methodClass.ClassName}.cs");
                }
            }

            {
                var startPrinter = new StatStartPrinter(_env.RootNamespaceName);

                foreach (var stat in topLevelStatTypes)
                {
                    Console.WriteLine($"Generating code for stat {stat.Name}");

                    startPrinter.ResetStat(stat);
                    startPrinter.WriteToFile($@"{_env.Paths.StatAutogenFolder}/{stat.Name}.cs");
                }
            }

            {
                (new SlotExtensionsPrinter(_env.RootNamespaceName, slots))
                    .WriteToFile(_env.Paths.SlotExtensionsPath);
            }

            foreach (var flag in flagEnums)
            {
                (new FlagsPrinter(flag))
                    .WriteToFile($"{_env.Paths.FlagsAutogenFolder}/{flag.ClassName}.cs");
            }

            {
                // They must live in at least the base namespace hopper
                if (_env.RootNamespaceName.Length >= "Hopper".Length)
                {
                    var mainPrinter = new AllInitPrinter()
                    {
                        components = componentWrappers,
                        behaviors = behaviorWrappers,
                        exportingClasses = exportingClasses,
                        methodClassInstances = methodClassInstances,
                        fieldsRequiringInit = fieldsRequiringInit,
                        staticIndentiyingStatFields = staticIndentiyingStatFields,
                        entityTypes = entityTypes,
                        statRootScope = _env.statParsingContext.currentScope,
                        slots = slots,
                        Namespace = _env.RootNamespaceName
                    };

                    Console.WriteLine("Generating code for the main init function");

                    File.WriteAllText(_env.Paths.MainAutogenFile, mainPrinter.TransformText(), Encoding.UTF8);
                }
                else
                {
                    Console.WriteLine($"Unable to generate code for the main init function: The common namespace between the files that participate in code generation must at least contain 'Hopper' (got '{_env.RootNamespaceName})'");
                }

            }
        }
    }

}