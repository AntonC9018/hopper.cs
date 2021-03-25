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

namespace Meta
{
    class Program
    {
        public static Task Main()
        {
            MSBuildLocator.RegisterDefaults();
            return Test4();
        }

        public static Task Test4()
        {
        {
            var t = new BehaviorEntityExtensions();
            t.Session = new Dictionary<string, object>();
            t.Session["behavior"] = new BehaviorInfo
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
            t.Session = new Dictionary<string, object>();
            t.Session["behavior"] = new BehaviorInfo
            {
                ClassName = "Acting",
                Namespace = "Hopper.Core.Components.Basic",
                ActivationAlias = "Act",
                Check = true
            };
            t.Session["chains"] = new ChainsInfo
            {
                ChainInfos = new ChainInfo[] {
                    new ChainInfo { Name = "Check" },
                    new ChainInfo { Name = "Do" }
                }
            };
            t.Session["context"] = new ContextInfo();
            t.Session["adapters"] = new HandlerAdapterInfo[] { 
                new HandlerAdapterInfo { HandlerName = "Hello" }
            };
            t.Session["presets"] = new PresetInfo[] { new PresetInfo { Name = "World" }};
            t.Initialize();
            Console.WriteLine(t.TransformText());
        }

            return Task.CompletedTask;
        }

        public static async Task Test3()
        {
            var msWorkspace = MSBuildWorkspace.Create();

            var solution = msWorkspace.CurrentSolution;
            solution.Workspace.WorkspaceFailed += (s, args) => Console.WriteLine(args.Diagnostic.Message);
            solution.Workspace.WorkspaceChanged += (s, args) => Console.WriteLine(args.ProjectId);

            var project = await msWorkspace.OpenProjectAsync("Meta.csproj");
            var compilation = await project.GetCompilationAsync();
            var document = project.Documents.FirstOrDefault();
            var syntaxTree = await document.GetSyntaxTreeAsync();
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var hello = (INamedTypeSymbol)compilation.GetSymbolsWithName("Hello").Single();
            var world = (INamedTypeSymbol)compilation.GetSymbolsWithName("World").Single();

            if (!SymbolEqualityComparer.Default.Equals(world.Interfaces.Single(), hello))
            {
                Console.WriteLine("Nope 1"); return;
            }

            var implementations = await SymbolFinder.FindImplementationsAsync(hello, solution);

            foreach (var d in compilation.GetDiagnostics())
            {
                Console.WriteLine(d);
            }

            if (!SymbolEqualityComparer.Default.Equals(implementations.Single(), world))
            {
                Console.WriteLine("Nope 2"); return;
            }
        }


        public static void Test2()
        {
            string solutionPath = @"../Core/Hopper_Core.csproj";
            MSBuildWorkspace msWorkspace;
            
            try
            {
                msWorkspace = MSBuildWorkspace.Create();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                return;
            }

            var solution = msWorkspace.CurrentSolution;
            solution.Workspace.WorkspaceFailed += (s, args) => Console.WriteLine(args.Diagnostic.Message);
            solution.Workspace.WorkspaceChanged += (s, args) => Console.WriteLine(args.ProjectId);

            var project = msWorkspace.OpenProjectAsync(solutionPath).Result;

            var compilation = project.GetCompilationAsync().Result;

            // Let's register mscorlib
            // compilation = compilation.AddReferences(
            //     MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

            // foreach (var f in Directory.EnumerateFiles(Path.GetDirectoryName(solutionPath), "bin/*.dll", SearchOption.AllDirectories))
            // {
            //     compilation = compilation.AddReferences(MetadataReference.CreateFromFile(f));
            // }

            // var document = project.Documents.FirstOrDefault(d => d.Name == "SomeClass.cs");
            // var syntaxTree = document.GetSyntaxTreeAsync().Result;
            // SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

            var ibehavior = (INamedTypeSymbol)compilation
                .GetSymbolsWithName("IBehavior", SymbolFilter.Type)
                .Single();
            var aliasAttribute = (INamedTypeSymbol)compilation
                .GetSymbolsWithName("AliasAttribute", SymbolFilter.Type)
                .Single();
            
            var implementations = SymbolFinder.FindReferencesAsync(ibehavior, solution).Result;

            Console.WriteLine(implementations.Count());

            foreach (var behavior in implementations)
            {
                Console.WriteLine(behavior.Locations.Single());
                foreach (var method in ((INamedTypeSymbol)behavior.Definition).GetMembers().OfType<IMethodSymbol>())
                foreach (var attrib in method.GetAttributes())
                {
                    if (SymbolEqualityComparer.Default.Equals(attrib.AttributeClass, aliasAttribute))
                    foreach (var arg in attrib.ConstructorArguments)
                    {
                        Console.WriteLine(arg);
                    }
                }
            }
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