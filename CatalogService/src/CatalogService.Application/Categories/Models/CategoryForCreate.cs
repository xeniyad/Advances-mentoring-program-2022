using CatalogService.Core.ValueObjects;

namespace CatalogService.Application.Categories.Models;

public sealed class CategoryForCreate
{
  public string Name { get; set; }
  public int? ParentId { get; set; }
  public Image? Image { get; set; }
}

