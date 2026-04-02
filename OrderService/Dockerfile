FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5003

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/OrderService.Core/OrderService.Core.csproj", "src/OrderService.Core/"]
COPY ["src/OrderService.Application/OrderService.Application.csproj", "src/OrderService.Application/"]
COPY ["src/OrderService.Infrastructure/OrderService.Infrastructure.csproj", "src/OrderService.Infrastructure/"]
COPY ["src/OrderService.Web/OrderService.Web.csproj", "src/OrderService.Web/"]
RUN dotnet restore "src/OrderService.Web/OrderService.Web.csproj"
COPY . .
WORKDIR "/src/src/OrderService.Web"
RUN dotnet build "OrderService.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderService.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:5003
ENTRYPOINT ["dotnet", "OrderService.Web.dll"]
