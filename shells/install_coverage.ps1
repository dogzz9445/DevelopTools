
dotnet tool install roslynator.dotnet.cli --tool-path ./nugets/tools
dotnet tool install coverlet.collectors --tool-path ./nugets/tools
dotnet tool install PowerShell --tool-path ./nugets/tools 
dotnet tool install sarif.multitool --tool-path ./nugets/tools
dotnet tool install coverlet.console --tool-path ./nugets/tools

# unittests
nugets\tools\coverlet "src/Apps/<project>/bin/Debug/net8.0/<project>.dll" --target "dotnet" --targetargs "test src/Apps/<project>/bin/Debug/net8.0/<project> --no-build"
cove
