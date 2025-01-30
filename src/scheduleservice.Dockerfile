FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./MedVisit.ScheduleService/MedVisit.ScheduleService.csproj ./MedVisit.ScheduleService/
COPY ./MedVisit.Core/MedVisit.Core.csproj ./MedVisit.Core/
RUN dotnet restore ./MedVisit.ScheduleService/MedVisit.ScheduleService.csproj

COPY ./MedVisit.ScheduleService/ ./MedVisit.ScheduleService/
COPY ./MedVisit.Core/ ./MedVisit.Core/
RUN dotnet publish ./MedVisit.ScheduleService/MedVisit.ScheduleService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MedVisit.ScheduleService.dll"]
