FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["OrderService/src/OrderService.Core/OrderService.Core.csproj", "OrderService/src/OrderService.Core/"]
COPY ["OrderService/src/OrderService.Application/OrderService.Application.csproj", "OrderService/src/OrderService.Application/"]
COPY ["OrderService/src/OrderService.Infrastructure/OrderService.Infrastructure.csproj", "OrderService/src/OrderService.Infrastructure/"]
COPY ["OrderService/src/OrderService.Web/OrderService.Web.csproj", "OrderService/src/OrderService.Web/"]
COPY ["ServiceDefaults/ServiceDefaults.csproj", "ServiceDefaults/"]
RUN dotnet restore "OrderService/src/OrderService.Web/OrderService.Web.csproj"
COPY OrderService/ OrderService/
COPY ServiceDefaults/ ServiceDefaults/
WORKDIR "/src/OrderService/src/OrderService.Web"
RUN dotnet build "OrderService.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderService.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "OrderService.Web.dll"]
