#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0.5 AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT Development
EXPOSE 5175

FROM mcr.microsoft.com/dotnet/sdk:6.0.300 AS build
WORKDIR /src
COPY ["deployment-history-backend/DeploymentHistoryBackend.csproj", "deployment-history-backend/"]
RUN dotnet restore "deployment-history-backend/DeploymentHistoryBackend.csproj"
COPY . .
WORKDIR "/src/deployment-history-backend"
RUN dotnet build "DeploymentHistoryBackend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DeploymentHistoryBackend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeploymentHistoryBackend.dll"]