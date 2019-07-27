FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["AgileVentures.TezPusher.Pusher/AgileVentures.TezPusher.Pusher.csproj", "AgileVentures.TezPusher.Pusher/"]
COPY ["AgileVentures.TezPusher.Model/AgileVentures.TezPusher.Model.csproj", "AgileVentures.TezPusher.Model/"]
RUN dotnet restore "AgileVentures.TezPusher.Pusher/AgileVentures.TezPusher.Pusher.csproj"
COPY . .
WORKDIR "/src/AgileVentures.TezPusher.Pusher"
RUN dotnet build "AgileVentures.TezPusher.Pusher.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AgileVentures.TezPusher.Pusher.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AgileVentures.TezPusher.Pusher.dll"]