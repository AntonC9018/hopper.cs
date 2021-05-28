@echo off
cd Meta
call dotnet run Hopper.Core;../Core/Hopper.Core.csproj Hopper.TestContent;../TestContent/Hopper.TestContent.csproj
cd ..