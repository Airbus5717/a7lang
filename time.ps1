echo "Debug build"
dotnet build
Measure-Command {  .\bin\Debug\net7.0\A7.exe | Out-Default }