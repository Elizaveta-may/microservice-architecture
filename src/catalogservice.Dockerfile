FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./MedVisit.CatalogService/MedVisit.CatalogService.csproj ./MedVisit.CatalogService/
COPY ./MedVisit.Core/MedVisit.Core.csproj ./MedVisit.Core/
RUN dotnet restore ./MedVisit.CatalogService/MedVisit.CatalogService.csproj

COPY ./MedVisit.CatalogService/ ./MedVisit.CatalogService/
COPY ./MedVisit.Core/ ./MedVisit.Core/
RUN dotnet publish ./MedVisit.CatalogService/MedVisit.CatalogService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MedVisit.CatalogService.dll"]
