
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["MyWebApplication.csproj", "./"]
RUN dotnet restore

COPY . .
WORKDIR "/src/."
RUN dotnet build "MyWebApplication.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyWebApplication.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyWebApplication.dll"]
