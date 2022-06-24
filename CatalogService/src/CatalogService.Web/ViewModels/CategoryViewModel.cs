using CatalogService.Core.ValueObjects;

namespace CatalogService.Web.ViewModels;

public class CategoryViewModel
{
  public int Id { get; set; }
  public string? Name { get; set; }
  public Image? Image { get; set; }
  public List<ItemViewModel> Items = new();
}
