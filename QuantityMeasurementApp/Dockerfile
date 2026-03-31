FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish QuantityMeasurementApp.APILayer/QuantityMeasurementApp.APILayer.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://+:${PORT:-5000}
ENTRYPOINT ["dotnet", "QuantityMeasurementApp.APILayer.dll"]