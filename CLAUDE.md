# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

This is a microservices-based e-commerce platform built on .NET 9. Services communicate asynchronously via Azure Service Bus and are orchestrated locally with .NET Aspire (AppHost). The full stack can also be run via Docker Compose.

## Services

| Service | Port | Directory |
|---|---|---|
| ApiGateway (YARP) | 5000 | `ApiGateway/` |
| CatalogService | 5001 | `CatalogService/` |
| CartingService | 5002 | `CartingService/` |
| OrderService | 5003 | `OrderService/` |
| Frontend (React/Vite) | 3000 | `frontend/` |

## Build & Run

### Local development with .NET Aspire
```bash
dotnet run --project AppHost
```
This starts all services with service discovery, health checks, and the Aspire dashboard.

### Docker Compose
```bash
docker-compose up --build
```

### Per-service
```bash
# CatalogService
cd CatalogService && dotnet build && dotnet run --project src/CatalogService.Web

# CartingService
cd CartingService && dotnet build && dotnet run --project CartingService.API

# OrderService
cd OrderService && dotnet build && dotnet run --project src/OrderService.Web

# ApiGateway
cd ApiGateway && dotnet build && dotnet run --project src/ApiGateway.Web
```

## Tests

```bash
# All tests for a service
dotnet test CatalogService/tests/CatalogService.UnitTests/CatalogService.UnitTests.csproj
dotnet test CartingService/CartingServiceTests/Carting.Tests.csproj
dotnet test OrderService/tests/OrderService.UnitTests/OrderService.UnitTests.csproj

# Single test
dotnet test --filter "FullyQualifiedName~TestMethodName"
```

Tests use xUnit with `WebApplicationFactory<Program>` for API-level integration tests and Moq for unit tests.

## Database Migrations (CatalogService)

```bash
cd CatalogService
dotnet ef migrations add <MigrationName> --startup-project src/CatalogService.Web --context AppDbContext --project src/CatalogService.Infrastructure
dotnet ef database update --startup-project src/CatalogService.Web --context AppDbContext --project src/CatalogService.Infrastructure
```

## Architecture

All services follow **Clean Architecture** with these layers (dependencies flow inward):

- **SharedKernel** — Base classes: `EntityBase` (holds domain events), `ValueObject`, `DomainEventBase`. Only in CatalogService; not shared across services as a package.
- **Core** — Domain aggregates, value objects, domain events, specification interfaces. No external dependencies.
- **Application** — DTOs and integration event definitions.
- **Infrastructure** — EF Core DbContext, repository implementations, Azure Service Bus client, Autofac module (`DefaultInfrastructureModule`).
- **Web** — ASP.NET Core controllers, HATEOAS resource wrappers, DI bootstrap via `Program.cs`.

CartingService is structured similarly but uses a `BL`/`DL`/`API` naming convention instead of the Clean Architecture layer names.

### Key Patterns

**Domain Events → Integration Events**
Domain events are accumulated on `EntityBase` and dispatched inside `AppDbContext.SaveChangesAsync` via `DomainEventDispatcher` (MediatR). Handlers then publish `ItemChangedIntegrationEvent` to the Azure Service Bus topic `"products"` (subscription `"catalogservice"`). CartingService subscribes and updates cart items when catalog items change.

**Specification Pattern**
Queries are expressed as `Specification<T>` objects (Ardalis.Specification + EF Core evaluator), avoiding raw LINQ in repositories.

**HATEOAS**
API responses are wrapped in `*Resource` classes derived from `ResourceBase`, with hypermedia links added by `*ResourceFactory` classes.

**Autofac DI**
`DefaultInfrastructureModule` in each service's Infrastructure layer wires up repositories and services, keeping `Program.cs` free of implementation references. All services use `builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())`.

**ServiceDefaults**
Shared project (`ServiceDefaults/`) added to every service via `builder.AddServiceDefaults()`. Configures OpenTelemetry, health check endpoints, and service discovery.

**API Versioning (CartingService)**
Uses header-based versioning (`Asp.Versioning`) with default version 2.0.

### Inter-service Communication

- **Async:** Azure Service Bus (topic/subscription). Connection string via `EventBusConnection` env var.
- **Sync:** OrderService calls CartingService via typed `HttpClient` with service discovery.
- **Routing:** ApiGateway (YARP) proxies all external traffic; routes configured in `appsettings.json` under `ReverseProxy`.

### Authentication

Azure AD B2C via `Microsoft.Identity.Web`. Configuration lives in `appsettings.json` under the `AzureAdB2C` section.

### Package Management

CatalogService uses `Directory.Packages.props` for centralized version management. Key packages: Ardalis suite, Autofac 9.x, Azure.Messaging.ServiceBus 7.x, EF Core 9.x, MediatR 12.x, Serilog 9.x.
