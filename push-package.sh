#!/bin/bash -e
dotnet pack src/AspNetCore.Authentication.Token/AspNetCore.Authentication.Token.csproj -c Release
dotnet nuget push src/AspNetCore.Authentication.Token/bin/Release/Beginor.AspNetCore.Authentication.Token.6.0.1.nupkg -s nuget.org -k $(cat ~/.nuget/key.txt)
rm src/AspNetCore.Authentication.Token/bin/Release/*.nupkg
