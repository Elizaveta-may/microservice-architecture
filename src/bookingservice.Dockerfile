FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./MedVisit.BookingService/MedVisit.BookingService.csproj ./MedVisit.BookingService/
COPY ./MedVisit.Core/MedVisit.Core.csproj ./MedVisit.Core/
RUN dotnet restore ./MedVisit.BookingService/MedVisit.BookingService.csproj

COPY ./MedVisit.BookingService/ ./MedVisit.BookingService/
COPY ./MedVisit.Core/ ./MedVisit.Core/
RUN dotnet publish ./MedVisit.BookingService/MedVisit.BookingService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MedVisit.BookingService.dll"]
