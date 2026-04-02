FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["CartingService/Carting.csproj", "CartingService/"]
COPY ["CartingService.API/Carting.API.csproj", "CartingService.API/"]
RUN dotnet restore "CartingService.API/Carting.API.csproj"
COPY . .
WORKDIR "/src/CartingService.API"
RUN dotnet build "Carting.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Carting.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:5002
ENTRYPOINT ["dotnet", "Carting.API.dll"]
