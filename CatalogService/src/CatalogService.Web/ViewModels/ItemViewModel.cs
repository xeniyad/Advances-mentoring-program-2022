using CatalogService.Core.ProjectAggregate;
using CatalogService.Core.ValueObjects;

namespace CatalogService.Web.ViewModels;

public class ItemViewModel
{
  public int Id { get; set; }
  public string? Title { get; set; }
  public string? Description { get; set; }
  public Image? Image { get; set; }
  public Money Price { get; set; }
  public uint Amount { get; set; }

  public static ItemViewModel FromItem(Item item)
  {
    return new ItemViewModel()
    {
      Id = item.Id,
      Title = item.Name,
      Description = item.Description,
      Image = string.IsNullOrEmpty(item.Image) ? null : new Image(item.Image),
      Price = item.Price,
      Amount = item.Amount
    };
  }
}
