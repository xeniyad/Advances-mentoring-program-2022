using CatalogService.Core.ProjectAggregate;
using CatalogService.Core.ValueObjects;

namespace CatalogService.Web.Endpoints.ProjectEndpoints;

public record ItemRecord(int Id, string Title, string? Description, string? Image, Category? Category, Money Price, uint Amount);
