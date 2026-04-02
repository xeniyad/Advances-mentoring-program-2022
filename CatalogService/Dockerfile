FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/CatalogService.SharedKernel/CatalogService.SharedKernel.csproj", "src/CatalogService.SharedKernel/"]
COPY ["src/CatalogService.Core/CatalogService.Core.csproj", "src/CatalogService.Core/"]
COPY ["src/CatalogService.Application/CatalogService.Application.csproj", "src/CatalogService.Application/"]
COPY ["src/CatalogService.Infrastructure/CatalogService.Infrastructure.csproj", "src/CatalogService.Infrastructure/"]
COPY ["src/CatalogService.Web/CatalogService.Web.csproj", "src/CatalogService.Web/"]
COPY ["Directory.Packages.props", "."]
RUN dotnet restore "src/CatalogService.Web/CatalogService.Web.csproj"
COPY . .
WORKDIR "/src/src/CatalogService.Web"
RUN dotnet build "CatalogService.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CatalogService.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:5001
ENTRYPOINT ["dotnet", "CatalogService.Web.dll"]
