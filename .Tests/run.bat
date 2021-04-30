dotnet build

if %1==all (
    nunit3-console bin\Debug\net4.8\Hopper.Tests.dll
) else (
    nunit3-console bin\Debug\net4.8\Hopper.Tests.dll --test=Hopper.Tests.%1
)