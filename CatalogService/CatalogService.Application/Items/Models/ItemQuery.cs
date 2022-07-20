using CatalogService.Core.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Application.Items.Models;

public sealed class ItemQuery : PagedQueryParams
{
  [FromQuery(Name = "order")]
  public string? Order { get; set; }
}
