FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["AgileVentures.TezPusher.Web/AgileVentures.TezPusher.Web.csproj", "AgileVentures.TezPusher.Web/"]
RUN dotnet restore "AgileVentures.TezPusher.Web/AgileVentures.TezPusher.Web.csproj"
COPY . .
WORKDIR "/src/AgileVentures.TezPusher.Web"
RUN dotnet build "AgileVentures.TezPusher.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AgileVentures.TezPusher.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AgileVentures.TezPusher.Web.dll"]