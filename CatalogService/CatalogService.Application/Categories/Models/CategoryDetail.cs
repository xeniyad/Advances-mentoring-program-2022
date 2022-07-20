using CatalogService.Application.Items.Models;
using CatalogService.Core.ValueObjects;

namespace CatalogService.Application.Categories.Models;

// ApiModel DTOs are used by ApiController classes and are typically kept in a side-by-side folder
public class CategoryDetail
{
  public int Id { get; set; }
  public string Name { get; set; }
  public int? ParentId { get; set; }
  public Image? Image { get; set; }
  public List<ItemDetail> Items { get; set; }
}

