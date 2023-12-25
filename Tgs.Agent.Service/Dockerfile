#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["Tgs.Agent.Service/Tgs.Agent.Service.csproj", "Tgs.Agent.Service/"]
COPY ["AgentRepositories/AgentRepositories.csproj", "AgentRepositories/"]
COPY ["Tgs.Agent.Core/Tgs.Agent.Core.csproj", "Tgs.Agent.Core/"]
RUN dotnet restore "./Tgs.Agent.Service/./Tgs.Agent.Service.csproj"
COPY . .
WORKDIR "/src/Tgs.Agent.Service"
RUN dotnet build "./Tgs.Agent.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.Agent.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.Agent.Service.dll"]