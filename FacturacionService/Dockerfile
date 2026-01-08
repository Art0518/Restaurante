# Build and publish the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
WORKDIR /src/FacturacionService
RUN dotnet restore FacturacionService.csproj
RUN dotnet publish FacturacionService.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Railway proporciona la variable PORT automáticamente
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "FacturacionService.dll"]
