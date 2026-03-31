FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy solution and all project files
COPY . .

# Restore and publish the API project
WORKDIR /app/QuantityMeasurementApp.APILayer
RUN dotnet publish -c Release -o /out

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /out .

# Render sets PORT env variable
ENV ASPNETCORE_URLS=http://+:${PORT:-5000}

ENTRYPOINT ["dotnet", "QuantityMeasurementApp.APILayer.dll"]
