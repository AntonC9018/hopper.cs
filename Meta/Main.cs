using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;

namespace Meta
{
    class Program
    {
        public static void Main()
        {
            Environment.SetEnvironmentVariable("MSBUILD_EXE_PATH", "C:/Program Files/dotnet/sdk/3.1.101/MSBuild.dll");

            string solutionPath = @"../Core/Hopper_Core.csproj";
            MSBuildWorkspace msWorkspace = null;
            
            try
            {
                msWorkspace = MSBuildWorkspace.Create();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }

            var solution = msWorkspace.CurrentSolution;
            solution.Workspace.WorkspaceFailed += (s, args) => Console.WriteLine(args.Diagnostic.Message);
            solution.Workspace.WorkspaceChanged += (s, args) => Console.WriteLine(args.ProjectId);

            var project = msWorkspace.OpenProjectAsync(solutionPath).Result;
            Console.WriteLine(project.DocumentIds.Count());

            var compilation = project.GetCompilationAsync().Result;

            // Let's register mscorlib
            compilation = compilation.AddReferences(
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            
            foreach (var f in Directory.EnumerateFiles(Path.GetDirectoryName(solutionPath), "*.dll"))
                compilation = compilation.AddReferences(MetadataReference.CreateFromFile(f));

            // var document = project.Documents.FirstOrDefault(d => d.Name == "SomeClass.cs");
            // var syntaxTree = document.GetSyntaxTreeAsync();
            // SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);

            foreach (var id in project.DocumentIds)
            {
                Console.WriteLine(project.GetDocument(id).FilePath);
            }


            var ibehavior = (INamedTypeSymbol)compilation
                .GetSymbolsWithName("Hopper.Core.Behaviors.IBehavior", SymbolFilter.Type)
                .Single();
            var aliasAttribute = (INamedTypeSymbol)compilation
                .GetSymbolsWithName("Hopper.Core.Behaviors.AliasAttribute", SymbolFilter.Type)
                .Single();
            var implementations = SymbolFinder.FindImplementationsAsync(ibehavior, solution).Result;

            foreach (var behavior in implementations.OfType<ITypeSymbol>())
            foreach (var method in behavior.GetMembers().OfType<IMethodSymbol>())
            foreach (var attrib in method.GetAttributes())
            {
                if (SymbolEqualityComparer.Default.Equals(attrib.AttributeClass, aliasAttribute))
                foreach (var arg in attrib.ConstructorArguments)
                {
                    Console.WriteLine(arg);
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