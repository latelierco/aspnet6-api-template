Template.AspNet.API

#how to package
1. dotnet pack nuget.csproj
2. dotnet new -i bin/Debug/AspNet6ApiTemplate.X.X.X.nupkg

#how to publish
1. dotnet nuget push bin/Debug/AspNet6ApiTemplate.X.X.X.nupkg -s https://api.nuget.org/v3/index.json --api-key xxxxxxxxxxxxx

#how to run 
1. dotnet new aspnet6-api -n Company.NextProject 
