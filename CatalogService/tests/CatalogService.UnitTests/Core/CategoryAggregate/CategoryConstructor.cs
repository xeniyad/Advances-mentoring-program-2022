using CatalogService.Core.ProjectAggregate;
using Xunit;

namespace CatalogService.UnitTests.Core.ProjectAggregate;

public class CategoryConstructor
{
  private string _testName = "test name";
  private Category? _testCategory;

  private Category CreateCategory()
  {
    return new Category(_testName);
  }

  [Fact]
  public void InitializesName()
  {
    _testCategory = CreateCategory();

    Assert.Equal(_testName, _testCategory.Name);
  }

  [Fact]
  public void InitializesItemsListToEmptyList()
  {
    _testCategory = CreateCategory();

    Assert.NotNull(_testCategory.Items);
  }

}
