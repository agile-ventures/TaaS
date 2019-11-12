#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["AgileVentures.TezPusher.ConsoleApp/AgileVentures.TezPusher.ConsoleApp.csproj", "AgileVentures.TezPusher.ConsoleApp/"]
COPY ["AgileVentures.TezPusher.Model/AgileVentures.TezPusher.Model.csproj", "AgileVentures.TezPusher.Model/"]
RUN dotnet restore "AgileVentures.TezPusher.ConsoleApp/AgileVentures.TezPusher.ConsoleApp.csproj"
COPY . .
WORKDIR "/src/AgileVentures.TezPusher.ConsoleApp"
RUN dotnet build "AgileVentures.TezPusher.ConsoleApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AgileVentures.TezPusher.ConsoleApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AgileVentures.TezPusher.ConsoleApp.dll"]