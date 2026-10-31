FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Web/FonoChampionHub.Web.csproj", "src/Web/"]
COPY ["src/Application/FonoChampionHub.Application.csproj", "src/Application/"]
COPY ["src/Domain/FonoChampionHub.Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/FonoChampionHub.Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/Web/FonoChampionHub.Web.csproj"

COPY src/ src/
RUN dotnet publish "src/Web/FonoChampionHub.Web.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FonoChampionHub.Web.dll"]
