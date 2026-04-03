FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY shared/Contracts/ shared/Contracts/
COPY auth-service/ auth-service/
RUN dotnet restore auth-service/AuthService.csproj
RUN dotnet build auth-service/AuthService.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish auth-service/AuthService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthService.dll"]
