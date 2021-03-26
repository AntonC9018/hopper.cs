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
            await Test4();
            // MSBuildLocator.RegisterDefaults();
            // msWorkspace = await InitWorkspace();
            // if (!failFlag)
            // {
            //     await Test2();
            // }
        }

        public static Task Test4()
        {
        {
            var t = new BehaviorEntityExtensions();
            t.behavior = new BehaviorInfo
            {
                ClassName = "Acting",
                Namespace = "Hopper.Core.Components.Basic",
                ActivationAlias = "Act",
                Check = true
            };
            t.Initialize();
            Console.WriteLine(t.TransformText());
        }
        {
            var t = new BehaviorPartial();
            t.behavior = new BehaviorInfo
            {
                ClassName = "Acting",
                Namespace = "Hopper.Core.Components.Basic",
                ActivationAlias = "Act",
                Check = true
            };
            t.chains = new ChainsInfo
            {
                ChainInfos = new ChainInfo[] {
                    new ChainInfo { Name = "Check" },
                    new ChainInfo { Name = "Do" }
                }
            };
            t.context = new ContextInfo();
            t.adapters = new HandlerAdapterInfo[] { 
                new HandlerAdapterInfo { HandlerName = "Hello" }
            };
            t.presets = new PresetInfo[] { new PresetInfo { Name = "World" }};
            t.Initialize();
            Console.WriteLine(t.TransformText());
        }

            return Task.CompletedTask;
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

        public static async Task Test2()
        {
            var solution = msWorkspace.CurrentSolution;
            var project = coreProject;
            var compilation = await project.GetCompilationAsync();

            var icomponent = (INamedTypeSymbol)compilation
                .GetTypeByMetadataName("Hopper.Core.Components.IComponent");
            var ibehavior = (INamedTypeSymbol)compilation
                .GetTypeByMetadataName("Hopper.Core.Components.IBehvaior");
            
            var aliasAttribute = (INamedTypeSymbol)compilation
                .GetTypeByMetadataName("Hopper.Core.Components.AliasAttribute");
            var autoActivationAttribute = (INamedTypeSymbol)compilation
                .GetTypeByMetadataName("Hopper.Core.Components.AutoActivationAttribute");
            var chainsAttribute = (INamedTypeSymbol)compilation
                .GetTypeByMetadataName("Hopper.Core.Components.ChainsAttribute");
            var injectAttribute = (INamedTypeSymbol)compilation
                .GetTypeByMetadataName("Hopper.Core.Components.InjectAttribute");
            var flagsAttribute = (INamedTypeSymbol)compilation
                .GetTypeByMetadataName("Hopper.Core.Components.FlagsAttribute");
            var exportAttribute = (INamedTypeSymbol)compilation
                .GetTypeByMetadataName("Hopper.Core.Components.ExportAttribute");
            
            var projectsToSearch = new HashSet<Project> {project};
            var implementations = await SymbolFinder.FindImplementationsAsync(
                icomponent, solution, transitive: false, projectsToSearch.ToImmutableHashSet());

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

        public static void Test()
        {
            Console.WriteLine("Started");

            using (FileStream fs = File.OpenRead("Code.cs"))
            {
                SyntaxTree tree = CSharpSyntaxTree.ParseText(SourceText.From(fs), path: "Code.cs");
                CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

                var compilation = CSharpCompilation.Create("HelloWorld")
                    .AddReferences(MetadataReference.CreateFromFile(typeof(string).Assembly.Location))
                    .AddSyntaxTrees(tree);
                
                var model = compilation.GetSemanticModel(tree);

                compilation.GetSymbolsWithName("Code", SymbolFilter.Type);

                var codeNamespace = root.Members.Single(
                    m => (m as NamespaceDeclarationSyntax)?.Name.ToString() == "Code")
                    as NamespaceDeclarationSyntax;

                var programClass = codeNamespace.Members.Single(
                    m => (m as ClassDeclarationSyntax)?.Identifier.ValueText == "Program") 
                    as ClassDeclarationSyntax;

                var method = programClass.Members.First(
                    mds => (mds as MethodDeclarationSyntax)?.Identifier.ValueText == "Thing")
                    as MethodDeclarationSyntax;

                var paramList = method.ParameterList.Parameters;
                Console.WriteLine("The Thing method has {0} parameters, and they are:", paramList.Count());
                
                foreach (var arg in paramList)
                {
                    Console.WriteLine("{0}, which is a {1} parameter", arg.Identifier, arg.Type);
                }

                var attribs = method.AttributeLists;

                foreach (var attrib in attribs.SelectMany(al => al.Attributes))
                foreach (var attribChildren in attrib.ArgumentList.ChildNodes())
                {
                    var argNode = attribChildren as AttributeArgumentSyntax;
                    foreach (var stringLiteral in argNode.ChildNodes().OfType<LiteralExpressionSyntax>())
                    {
                        var name = model.GetConstantValue(stringLiteral).Value?.ToString();
                        Console.WriteLine(name); // Test1 
                                                // Test2
                    }
                }
            }
        }
    }
}