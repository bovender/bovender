REM nuget pack Bovender.csproj -Build -Symbols -Properties Configuration=Release
nuget pack Bovender.csproj -Build -Symbols -Properties Configuration=Debug

REM Once two-digit version numbers are reached, this must be adjusted
REM (don't see another way to specify the file names with wildcards)
nuget push Bovender.?.??.?.0.nupkg
nuget push Bovender.?.??.?.0.symbols.nupkg

pause
