#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AgentPortalAPI.Service/nuget.config", "AgentPortalAPI.Service/"]
COPY ["AgentPortalAPI.Service/AgentPortalAPI.Service.csproj", "AgentPortalAPI.Service/"]
COPY ["AgentPortalAPI.Accessors/AgentPortalAPI.Accessors.csproj", "AgentPortalAPI.Accessors/"]
COPY ["AgentPortalAPI.Managers/AgentPortalAPI.Managers.csproj", "AgentPortalAPI.Managers/"]
COPY ["AgentPortalAPI.Utilities/AgentPortalAPI.Utilities.csproj", "AgentPortalAPI.Utilities/"]
RUN dotnet restore "AgentPortalAPI.Service/AgentPortalAPI.Service.csproj" --configfile "./AgentPortalAPI.Service/nuget.config"
COPY . .
WORKDIR "/src/AgentPortalAPI.Service"
RUN dotnet build "AgentPortalAPI.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AgentPortalAPI.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AgentPortalAPI.Service.dll"]