#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["deployments-history-backend/deployments-history-backend.csproj", "deployments-history-backend/"]
RUN dotnet restore "deployments-history-backend/deployments-history-backend.csproj"
COPY . .
WORKDIR "/src/deployments-history-backend"
RUN dotnet build "deployments-history-backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "deployments-history-backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "deployments-history-backend.dll"]