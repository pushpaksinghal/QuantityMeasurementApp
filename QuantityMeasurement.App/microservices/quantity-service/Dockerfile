FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY shared/Contracts/ shared/Contracts/
COPY quantity-service/ quantity-service/
RUN dotnet restore quantity-service/QuantityService.csproj
RUN dotnet build quantity-service/QuantityService.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish quantity-service/QuantityService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuantityService.dll"]
