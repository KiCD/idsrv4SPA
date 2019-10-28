FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["TB.RestAPI/TB.RestAPI.csproj", "TB.RestAPI/"]
RUN dotnet restore "TB.RestAPI/TB.RestAPI.csproj"
COPY . .
WORKDIR "/src/TB.RestAPI"
RUN dotnet build "TB.RestAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TB.RestAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TB.RestAPI.dll"]