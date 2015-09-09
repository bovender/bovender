REM nuget pack Bovender.csproj -Build -Symbols -Properties Configuration=Release
nuget pack Bovender.csproj -Build -Symbols -Properties Configuration=Debug
nuget push Bovender*.nupkg
