FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln .
COPY *.csproj ./app/
COPY ./ ./app/
WORKDIR /src/app
RUN dotnet publish -c release

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /src/app
COPY --from=build /src/app ./
ENTRYPOINT ["dotnet", "./bin/Debug/net8.0/AspNetCoreGroup.UserFunctionality.IdentityServer.dll"]