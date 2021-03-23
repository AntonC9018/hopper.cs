using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Meta
{
    class Program
    {
        public static void Main(string[] args)
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