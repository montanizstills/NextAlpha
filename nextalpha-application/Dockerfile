# Build context is this directory (nextalpha-application/). From the repo root:
#   docker build -t nextalpha-api nextalpha-application
#   docker run --rm -p 8080:80 nextalpha-api     # then: curl http://localhost:8080/health
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NETWebAppMVC.csproj", "./"]
RUN dotnet restore "NETWebAppMVC.csproj"
COPY . .
RUN dotnet build "NETWebAppMVC.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "NETWebAppMVC.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NETWebAppMVC.dll"]
