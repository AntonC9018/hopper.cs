@echo off
cd .Tests
call dotnet build
cd ..

if %1==all (
    nunit3-console build\bin\Debug\net4.8\Hopper.Tests.dll
) else (
    nunit3-console build\bin\Debug\net4.8\Hopper.Tests.dll --test=Hopper.Tests.%1
)