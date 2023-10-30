dotnet clean
dotnet restore
cd ./src/NativeBuffering
dotnet pack -c Release -o ../../packages
cd ../../
cd ./src/NativeBuffering.Generator
dotnet pack -c Release -o ../../packages
pause