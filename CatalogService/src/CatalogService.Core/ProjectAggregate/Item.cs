using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CatalogService.Core.ValueObjects;
using CatalogService.SharedKernel;
using CatalogService.SharedKernel.Interfaces;

namespace CatalogService.Core.ProjectAggregate;

public class Item : EntityBase
{
  public Item() { }
  public Item(string name, string? description, string? image, Money price, uint amount)
  {
    Name = name;
    Description = description;
    Image = image;
    Price = price;
    Amount = amount;
  }

  public string Name { get; set; }
  
  public string? Description { get; set; }
  public string? Image { get; set; }
  public Money Price { get; set; }
  public uint Amount { get; set; }
  public Category Category { get; set; }
}
