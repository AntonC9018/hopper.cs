Consider 3 projects:
- project `A` referencing nothing;
- project `B` referencing project `A`;
- project `C` referencing both `A` and `B`.

`ProjectsTest/A/A.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <TargetFramework>net4.8</TargetFramework>
        <OutputType>Library</OutputType>
    </PropertyGroup>

</Project>
```

`ProjectsTest/A/Main.cs`:
```C#
namespace A
{
    public class Class{}
}
```

`ProjectsTest/B/B.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <TargetFramework>net4.8</TargetFramework>
        <OutputType>Library</OutputType>
    </PropertyGroup>

    <ItemGroup>
        <ProjectReference Include="..\A\A.csproj"></ProjectReference>
    </ItemGroup>

</Project>
```

`ProjectsTest/B/Main.cs`:
```C#
namespace B
{
    public class Class{}
}
```

`ProjectsTest/C/C.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <TargetFramework>net4.8</TargetFramework>
        <OutputType>Library</OutputType>
    </PropertyGroup>

    <ItemGroup>
        <ProjectReference Include="..\A\A.csproj"></ProjectReference>
        <ProjectReference Include="..\B\B.csproj"></ProjectReference>
    </ItemGroup>

</Project>
```

`ProjectsTest/C/Main.cs`:
```C#
namespace C
{
    public class Class{}
}
```

The 4th is called `Mine` and exists for the purposes of testing Roslyn code. 
If you simply copy the code below it should compile for you.

`Mine/Mine.csproj`:
```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <StartupObject>Mine.Program</StartupObject>
        <TargetFramework>net4.8</TargetFramework>
    </PropertyGroup>
    
    <ItemGroup>
        <!-- Prevents copying msbuild dlls into the output directory. Instead, locate the ones on system -->
        <PackageReference Include="Microsoft.Build" Version="15.3.409" ExcludeAssets="runtime" />
        <PackageReference Include="Microsoft.Build.Utilities.Core" Version="15.3.409" ExcludeAssets="runtime" />
        <PackageReference Include="Microsoft.Build.Locator" Version="1.1.2" />
        <PackageReference Include="Microsoft.VisualStudio.TextTemplating.14.0" Version="14.3.25407" />
    </ItemGroup>

    <ItemGroup>
        <PackageReference Include="Microsoft.CodeAnalysis" Version="3.9.0" />
        <PackageReference Include="Microsoft.CodeAnalysis.Workspaces.Common" Version="3.9.0" />
        <PackageReference Include="Microsoft.CodeAnalysis.Workspaces.MSBuild" Version="3.9.0" />
    </ItemGroup>

</Project>
```

`Mine/Main.cs`:
```C#
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
            var projectA = await msWorkspace.OpenProjectAsync("../ProjectsTest/A/A.csproj");
            var projectB = await msWorkspace.OpenProjectAsync("../ProjectsTest/B/B.csproj");
            var projectC = await msWorkspace.OpenProjectAsync("../ProjectsTest/C/C.csproj");

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
```

Running the program with `dotnet run` prints the class names, but when comparing symbols taken from different compilations, it evaluates to `false` for all of them.

At first I figured this was because, when loading the project `B` with `OpenProjectAsync()`, the project `A` gets loaded the second time. 
Same for `C`: when it is loaded, it loads the project `A` the third time, then loads `B` the second time, which in turn loads `A` again?
But that theory doesn't make much sense since they wouldn't make it that inefficient.

Also, it is in fact proven wrong by changing the order of loading the projects, from `A`, `B`, `C` to `C`, `A`, `B`:
```C#
var projectA = await msWorkspace.OpenProjectAsync("../ProjectsTest/A/A.csproj");
var projectB = await msWorkspace.OpenProjectAsync("../ProjectsTest/B/B.csproj");
var projectC = await msWorkspace.OpenProjectAsync("../ProjectsTest/C/C.csproj");
```

Running this code throws an error and prints:
```
Warning while opening a project:
  Found project reference without a matching metadata reference: H:\hopper_godot\Hopper\ProjectsTest\A\A.csproj
Warning while opening a project:
  Found project reference without a matching metadata reference: H:\hopper_godot\Hopper\ProjectsTest\B\B.csproj

Unhandled Exception: System.ArgumentException: 'A' is already part of the workspace.
```

What I need is for those symbols to all be equal. 
Perhaps taking them from the different compilations is not correct.
Perhaps there is a way of sort of merging these compilations, or sort of looking at the entire solution's "type space", or getting references to individual projects by path or by name having the solution.

To be clear: making use of just the project `C`'s compilation will work in this case, because it references all projects from before, but not in general.
If `C` doesn't reference `B`, then you cannot get `B.Class` from `C`'s compilation.
So this would be just a workaround.

## Some more context

I'm working on a game that will have different mods.
Mods may reference the game's core as well as other mods.
I generate some code for the mods, using Roslyn and T4.

So what I'd doing is first loading the Core project and storing the relevant symbols that I care about globally.
Then I load each of the mods, do some code analysis and then generate some code for each of them.

I could analyze the code for each of the mod projects separately, setting up the context first by analyzing each of the referenced mods, but that seems like a waste of 