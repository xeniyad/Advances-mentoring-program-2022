using CatalogService.Core.ProjectAggregate;
using Xunit;

namespace CatalogService.UnitTests.Core.ProjectAggregate;

public class CategoryClassTest
{
  private Category _testCategory = new Category("some name", null, null);

  [Fact]
  public void AddsItemToItems()
  {
    var _testItem = new Item
    {
      Name = "title",
      Description = "description"
    };

    _testCategory.AddItem(_testItem);

    Assert.Contains(_testItem, _testCategory.Items);
  }

  [Fact]
  public void ThrowsExceptionGivenNullItem()
  {
#nullable disable
    Action action = () => _testCategory.AddItem(null);
#nullable enable

    var ex = Assert.Throws<ArgumentNullException>(action);
    Assert.Equal("newItem", ex.ParamName);
  }
}
