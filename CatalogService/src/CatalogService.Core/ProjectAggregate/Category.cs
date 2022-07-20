using Ardalis.GuardClauses;
using CatalogService.SharedKernel;
using CatalogService.SharedKernel.Interfaces;

namespace CatalogService.Core.ProjectAggregate;

public class Category : EntityBase
{
  public string Name { get; private set; }
  public string? Image { get; private set; }
  public int? ParentId { get; set; }
  public List<Item>? Items { get; set; }

  public Category(int id, string name, int? parentId = null, string? image = null) {

    Id = id;
    Name = Guard.Against.NullOrEmpty(name, nameof(name));
    Name = Guard.Against.NullOrEmpty(name, nameof(name));
    ParentId = parentId;
    Image = image;
  }

  public Category(string name, int? parentId = null, string? image = null)
  {
    Name = Guard.Against.NullOrEmpty(name, nameof(name));
    ParentId = parentId;
    Image = image;
    Items = new List<Item>();
  }

  public Category() { }

  public void AddItem(Item newItem)
  {
    Guard.Against.Null(newItem, nameof(newItem));
    Items.Add(newItem);
  }

  public void UpdateName(string name)
  {
    Name = Guard.Against.NullOrEmpty(name, nameof(name));
  }
}
