FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./MedVisit.PaymentService/MedVisit.PaymentService.csproj ./MedVisit.PaymentService/
RUN dotnet restore ./MedVisit.PaymentService/MedVisit.PaymentService.csproj

COPY ./MedVisit.PaymentService/ ./MedVisit.PaymentService/
RUN dotnet publish ./MedVisit.PaymentService/MedVisit.PaymentService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MedVisit.PaymentService.dll"]
