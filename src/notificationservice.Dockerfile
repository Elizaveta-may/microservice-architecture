FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./MedVisit.NotificationService/MedVisit.NotificationService.csproj ./MedVisit.NotificationService/
COPY ./MedVisit.Core/MedVisit.Core.csproj ./MedVisit.Core/
RUN dotnet restore ./MedVisit.NotificationService/MedVisit.NotificationService.csproj

COPY ./MedVisit.NotificationService/ ./MedVisit.NotificationService/
COPY ./MedVisit.Core/ ./MedVisit.Core/
RUN dotnet publish ./MedVisit.NotificationService/MedVisit.NotificationService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MedVisit.NotificationService.dll"]
