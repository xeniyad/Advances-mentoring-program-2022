using System.ComponentModel.DataAnnotations;
using CatalogService.Core.ProjectAggregate;
using CatalogService.Core.ValueObjects;

namespace CatalogService.Application.Items.Models;

// ApiModel DTOs are used by ApiController classes and are typically kept in a side-by-side folder
public class ItemDetail
{
  public int Id { get; set; }
  [Required]
  public string? Name { get; set; }
  public string? Description { get; set; }
  public Image? Image { get; set; }
  public Money Price { get; set; }
  public uint Amount { get; set; }

  public static ItemDetail FromItem(Item item)
  {
    return new ItemDetail()
    {
      Id = item.Id,
      Name = item.Name,
      Image = new Image(item.Image),
      Description = item.Description,
      Price = item.Price,
      Amount = item.Amount
    };
  }
}
