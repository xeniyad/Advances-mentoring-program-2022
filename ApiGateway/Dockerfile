FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ApiGateway/src/ApiGateway.Web/ApiGateway.Web.csproj", "ApiGateway/src/ApiGateway.Web/"]
COPY ["ServiceDefaults/ServiceDefaults.csproj", "ServiceDefaults/"]
RUN dotnet restore "ApiGateway/src/ApiGateway.Web/ApiGateway.Web.csproj"
COPY ApiGateway/ ApiGateway/
COPY ServiceDefaults/ ServiceDefaults/
WORKDIR "/src/ApiGateway/src/ApiGateway.Web"
RUN dotnet build "ApiGateway.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiGateway.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "ApiGateway.Web.dll"]
