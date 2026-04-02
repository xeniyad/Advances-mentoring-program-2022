# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build

# Run all tests
dotnet test

# Run a single test project
dotnet test tests/CatalogService.UnitTests/CatalogService.UnitTests.csproj

# Run the web app
dotnet run --project src/CatalogService.Web

# Add EF Core migration (from repo root)
dotnet ef migrations add <MigrationName> --startup-project src/CatalogService.Web --context AppDbContext --project src/CatalogService.Infrastructure

# Apply migrations
dotnet ef database update --startup-project src/CatalogService.Web --context AppDbContext --project src/CatalogService.Infrastructure
```

## Architecture

This is a **Clean Architecture** microservice for managing a product catalog (Categories and Items). The dependency rule flows inward: Web → Infrastructure → Core ← SharedKernel.

### Layer responsibilities

- **CatalogService.SharedKernel** — Base classes (`EntityBase`, `ValueObject`, `DomainEventBase`, `DomainEventDispatcher`) and pagination types. Intended to be shared across bounded contexts.
- **CatalogService.Core** — Domain aggregates (`Category`, `Item`), value objects (`Money`, `Image`), domain events (`NewItemAddedEvent`), specifications, and service interfaces (`ICategoryService`, `IItemService`, `IEventBus`). No external dependencies.
- **CatalogService.Application** — DTOs for create/update/read operations and integration event definitions (`ItemChangedIntegrationEvent`).
- **CatalogService.Infrastructure** — EF Core `AppDbContext` (SQL Server), service implementations, Azure Service Bus (`EventBusServiceBus`), Autofac DI module (`DefaultInfrastructureModule`).
- **CatalogService.Web** — ASP.NET Core REST API. Controllers live in `Api/`, resource/HATEOAS models in `Models/`, DI/middleware setup in `Setup/`, integration event publishing in `Integration/`.

### Key patterns

- **Domain Events**: Entities accumulate events via `EntityBase`. `AppDbContext.SaveChangesAsync` dispatches them through `DomainEventDispatcher` (MediatR). Integration events are then published to Azure Service Bus.
- **Specification Pattern**: Queries are expressed as `Specification<T>` objects (Ardalis.Specification + EF Core) rather than raw LINQ in services.
- **Resource/HATEOAS**: API responses are wrapped in resource objects with hypermedia links via `ResourceBase` and `*ResourceFactory` classes.
- **Autofac modules**: Infrastructure wiring is done in `DefaultInfrastructureModule`, keeping `Program.cs` free of implementation references.

### Integration events

`ItemsController` publishes `ItemChangedIntegrationEvent` to Azure Service Bus when an item's name or price changes. The service bus topic is `products`, subscription `catalogservice`. If no Service Bus connection string is configured, publishing is skipped.

### Database

Primary: SQL Server (`LocalDB` in development). SQLite is available as an alternative — switch the connection string in `appsettings.json` and change `UseSqlServer` → `UseSqlite` in `StartupSetup.cs`. Database is seeded on startup via `SeedData`.

### Authentication

Azure AD B2C via `Microsoft.Identity.Web`. Configuration is in `appsettings.json` under `AzureAdB2C`.

### Packages

Centrally managed in `Directory.Packages.props`. Key libraries: Ardalis.Specification, Ardalis.GuardClauses, Ardalis.Result, MediatR, Autofac, Serilog, Swashbuckle.
