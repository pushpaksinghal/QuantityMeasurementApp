FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY shared/Contracts/ shared/Contracts/
COPY history-service/ history-service/
RUN dotnet restore history-service/HistoryService.csproj
RUN dotnet build history-service/HistoryService.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish history-service/HistoryService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HistoryService.dll"]
