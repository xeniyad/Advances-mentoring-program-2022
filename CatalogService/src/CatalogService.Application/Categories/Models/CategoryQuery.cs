using CatalogService.Core.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Core.ProjectAggregate.Queries;

public sealed class CategoryQuery : PagedQueryParams
  {
      [FromQuery(Name = "order")]
      public string? Order { get; set; }
  }
