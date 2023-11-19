#!/bin/bash -e
VERSION="8.0.0"
dotnet pack src/AspNetCore.Authentication.Token/AspNetCore.Authentication.Token.csproj -c Release
dotnet nuget push src/AspNetCore.Authentication.Token/bin/Release/Beginor.AspNetCore.Authentication.Token.$VERSION.nupkg -s nuget.org -k $(cat ~/.nuget/key.txt)
rm src/AspNetCore.Authentication.Token/bin/Release/*.nupkg
