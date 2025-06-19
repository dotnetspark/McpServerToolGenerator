$ErrorActionPreference = "Stop"

Write-Host "Clean all projects"
dotnet clean

Write-Host "Clear all NuGet caches"
dotnet nuget locals all --clear

Write-Host "Remove all bin and obj folders recursively"
Get-ChildItem -Recurse -Directory -Include bin,obj | Remove-Item -Recurse -Force

Write-Host "Remove all packages from packages directory"
if (Test-Path "packages") {
    Remove-Item "packages\*" -Recurse -Force
}

Write-Host "Build and pack FastTrack.Common"
dotnet build src/FastTrack.Common/FastTrack.Common.csproj --configuration Release
dotnet pack src/FastTrack.Common/FastTrack.Common.csproj --configuration Release --output packages

Write-Host "Build and pack FastTrack"
dotnet build src/FastTrack/FastTrack.csproj --configuration Release
dotnet pack src/FastTrack/FastTrack.csproj --configuration Release --output packages

Write-Host "Restore and build CalculatorConsole"
dotnet restore samples/CalculatorConsole/CalculatorConsole.csproj
dotnet build samples/CalculatorConsole/CalculatorConsole.csproj