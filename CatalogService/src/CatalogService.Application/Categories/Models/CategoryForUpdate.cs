using CatalogService.Core.ValueObjects;

namespace CatalogService.Application.Categories.Models;
public sealed class CategoryForUpdate
{
  public int Id { get; set; }
  public string Name { get; set; }
  public int? ParentId { get; set; }
  public Image? Image { get; set; }
}
