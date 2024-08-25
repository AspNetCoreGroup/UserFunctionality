FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Development
WORKDIR /src
COPY ["BackendService/BackendService.csproj", "BackendService/"]
COPY ["BackendCommonLibrary/BackendCommonLibrary.csproj", "BackendCommonLibrary/"]
RUN dotnet restore "./BackendService/BackendService.csproj"
COPY . .
WORKDIR "/src/BackendService"
RUN dotnet build "./BackendService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Development
RUN dotnet publish "./BackendService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackendService.dll"]