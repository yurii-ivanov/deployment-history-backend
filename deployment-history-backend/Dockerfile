#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["deployment-history-backend/deployment-history-backend.csproj", "deployment-history-backend/"]
RUN dotnet restore "deployment-history-backend/deployment-history-backend.csproj"
COPY . .
WORKDIR "/src/deployment-history-backend"
RUN dotnet build "deployment-history-backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "deployment-history-backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "deployment-history-backend.dll"]