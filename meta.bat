@echo off

cd Tests
call dotnet restore
cd ..

cd Meta
call dotnet run --configuration Release Hopper.Core;../Core/Hopper.Core.csproj Hopper.TestContent;../TestContent/Hopper.TestContent.csproj
cd ..