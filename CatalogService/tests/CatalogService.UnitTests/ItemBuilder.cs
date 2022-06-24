using CatalogService.Core.ProjectAggregate;

namespace CatalogService.UnitTests;

// Learn more about test builders:
// https://ardalis.com/improve-tests-with-the-builder-pattern-for-test-data
public class ItemBuilder
{
  private Item _item = new Item();

  public ItemBuilder Id(int id)
  {
    _item.Id = id;
    return this;
  }

  public ItemBuilder Name(string title)
  {
    _item.Name = title;
    return this;
  }

  public ItemBuilder Description(string description)
  {
    _item.Description = description;
    return this;
  }

  public ItemBuilder WithDefaultValues()
  {
    _item = new Item() { Id = 1, Name = "Test Item", Description = "Test Description" };

    return this;
  }

  public Item Build() => _item;
}
