#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["TB.Entities/TB.Entities.csproj", "TB.Entities/"]
COPY ["TB.TokenService/TB.TokenService.csproj", "TB.TokenService/"]
RUN dotnet restore "TB.TokenService/TB.TokenService.csproj"
COPY . .
WORKDIR "/src/TB.TokenService"
RUN dotnet build "TB.TokenService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TB.TokenService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TB.TokenService.dll"]