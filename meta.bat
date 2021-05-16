@echo off
cd Meta
call dotnet run ../Core/Hopper.Core.csproj ../TestContent/Hopper.TestContent.csproj
cd ..