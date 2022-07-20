using System.ComponentModel.DataAnnotations;
using CatalogService.Core.ValueObjects;

namespace CatalogService.Application.Items.Models;

public sealed class ItemForCreate
{
  [Required]
  public string? Name { get; set; }
  public string? Description { get; set; }
  public Image? Image { get; set; }
  public Money Price { get; set; }
  public uint Amount { get; set; }
  public int CategoryId { get; set; }
}

