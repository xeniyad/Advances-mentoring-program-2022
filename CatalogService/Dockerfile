FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["CatalogService/src/CatalogService.SharedKernel/CatalogService.SharedKernel.csproj", "CatalogService/src/CatalogService.SharedKernel/"]
COPY ["CatalogService/src/CatalogService.Core/CatalogService.Core.csproj", "CatalogService/src/CatalogService.Core/"]
COPY ["CatalogService/src/CatalogService.Application/CatalogService.Application.csproj", "CatalogService/src/CatalogService.Application/"]
COPY ["CatalogService/src/CatalogService.Infrastructure/CatalogService.Infrastructure.csproj", "CatalogService/src/CatalogService.Infrastructure/"]
COPY ["CatalogService/src/CatalogService.Web/CatalogService.Web.csproj", "CatalogService/src/CatalogService.Web/"]
COPY ["CatalogService/Directory.Packages.props", "CatalogService/"]
COPY ["ServiceDefaults/ServiceDefaults.csproj", "ServiceDefaults/"]
RUN dotnet restore "CatalogService/src/CatalogService.Web/CatalogService.Web.csproj"
COPY CatalogService/ CatalogService/
COPY ServiceDefaults/ ServiceDefaults/
WORKDIR "/src/CatalogService/src/CatalogService.Web"
RUN dotnet build "CatalogService.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CatalogService.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "CatalogService.Web.dll"]
