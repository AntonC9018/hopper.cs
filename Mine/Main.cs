using System;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace Mine
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            MSBuildLocator.RegisterDefaults();
            await Test();
        }

        public static async Task Test()
        {
            MSBuildWorkspace msWorkspace = MSBuildWorkspace.Create();

            msWorkspace.WorkspaceFailed += (s, args) => 
            {
                if (args.Diagnostic.Kind == WorkspaceDiagnosticKind.Failure)
                {
                    Console.WriteLine($"Unable to open the project.\n  {args.Diagnostic.Message}");
                }
                else
                {
                    Console.WriteLine($"Warning while opening a project:\n  {args.Diagnostic.Message}");
                }
            };

            msWorkspace.LoadMetadataForReferencedProjects = true;
            var projectC = await msWorkspace.OpenProjectAsync("../ProjectsTest/C/C.csproj");
            var projectA = await msWorkspace.OpenProjectAsync("../ProjectsTest/A/A.csproj");
            var projectB = await msWorkspace.OpenProjectAsync("../ProjectsTest/B/B.csproj");

            var compilationA  = await projectA.GetCompilationAsync();
            var symbolClassAA = compilationA.GetTypeByMetadataName("A.Class");
            
            var compilationB  = await projectB.GetCompilationAsync();
            var symbolClassAB = compilationB.GetTypeByMetadataName("A.Class");
            var symbolClassBB = compilationB.GetTypeByMetadataName("B.Class");

            var compilationC  = await projectC.GetCompilationAsync();
            var symbolClassAC = compilationC.GetTypeByMetadataName("A.Class");
            var symbolClassBC = compilationC.GetTypeByMetadataName("B.Class");

            
            // Prints the metadata names of the types.
            // If the classes were not found, it would print empty strings instead of their metadata names.
            Console.WriteLine($"{nameof(symbolClassAA)} = {symbolClassAA}");
            Console.WriteLine($"{nameof(symbolClassAB)} = {symbolClassAB}");
            Console.WriteLine($"{nameof(symbolClassAC)} = {symbolClassAC}");
            Console.WriteLine($"{nameof(symbolClassBB)} = {symbolClassBB}");
            Console.WriteLine($"{nameof(symbolClassBC)} = {symbolClassBC}");
            Console.WriteLine($"");

            // Compile symbols from different compilations.
            Console.WriteLine($"{nameof(symbolClassAA)} = {nameof(symbolClassAB)}? {symbolClassAA == symbolClassAB}");
            Console.WriteLine($"{nameof(symbolClassAA)} = {nameof(symbolClassAC)}? {symbolClassAA == symbolClassAC}");
            Console.WriteLine($"{nameof(symbolClassAB)} = {nameof(symbolClassAC)}? {symbolClassAA == symbolClassAC}");
            Console.WriteLine($"{nameof(symbolClassBC)} = {nameof(symbolClassBB)}? {symbolClassBC == symbolClassBB}");
            
            // False
            // False
            // False
            // False
        }
    }
}