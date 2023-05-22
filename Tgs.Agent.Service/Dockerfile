#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["Tgs.Agent.Service/Tgs.Agent.Service.csproj", "Tgs.Agent.Service/"]
COPY ["Tgs.Agent.Core/Tgs.Agent.Core.csproj", "Tgs.Agent.Core/"]
RUN dotnet restore "Tgs.Agent.Service/Tgs.Agent.Service.csproj"
COPY . .
WORKDIR "/src/Tgs.Agent.Service"
RUN dotnet build "Tgs.Agent.Service.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.Agent.Service.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.Agent.Service.dll"]