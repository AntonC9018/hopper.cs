dotnet build

if %1==all (
    nunit3-console bin\Debug\net45\Hopper_Tests.dll
) else (
    nunit3-console bin\Debug\net45\Hopper_Tests.dll --test=Hopper.Tests.%1
)