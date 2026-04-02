FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/ApiGateway.Web/ApiGateway.Web.csproj", "src/ApiGateway.Web/"]
RUN dotnet restore "src/ApiGateway.Web/ApiGateway.Web.csproj"
COPY . .
WORKDIR "/src/src/ApiGateway.Web"
RUN dotnet build "ApiGateway.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiGateway.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "ApiGateway.Web.dll"]
