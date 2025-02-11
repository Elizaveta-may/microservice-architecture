FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./MedVisit.AccessManagement/MedVisit.AccessManagement.csproj ./MedVisit.AccessManagement/
COPY ./MedVisit.Core/MedVisit.Core.csproj ./MedVisit.Core/
COPY ./MedVisit.Common.AuthDbContext/MedVisit.Common.AuthDbContext.csproj ./MedVisit.Common.AuthDbContext/
RUN dotnet restore ./MedVisit.AccessManagement/MedVisit.AccessManagement.csproj

COPY ./MedVisit.AccessManagement/ ./MedVisit.AccessManagement/
COPY ./MedVisit.Core/ ./MedVisit.Core/
COPY ./MedVisit.Common.AuthDbContext/ ./MedVisit.Common.AuthDbContext/
RUN dotnet publish ./MedVisit.AccessManagement/MedVisit.AccessManagement.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MedVisit.AccessManagement.dll"]
