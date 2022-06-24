using CatalogService.Core.ProjectAggregate;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CatalogService.IntegrationTests.Data;

public class EfRepositoryUpdate : BaseEfRepoTestFixture
{
  [Fact]
  public async Task UpdatesItemAfterAddingIt()
  {
    // add a project
    var repository = GetRepository();
    var initialName = Guid.NewGuid().ToString();
    var category = new Category(initialName);

    await repository.AddAsync(category);

    // detach the item so we get a different instance
    _dbContext.Entry(category).State = EntityState.Detached;

    // fetch the item and update its title
    var newCategory = (await repository.ListAsync())
        .FirstOrDefault(project => project.Name == initialName);
    if (newCategory == null)
    {
      Assert.NotNull(newCategory);
      return;
    }
    Assert.NotSame(category, newCategory);
    var newName = Guid.NewGuid().ToString();
    newCategory.UpdateName(newName);

    // Update the item
    await repository.UpdateAsync(newCategory);

    // Fetch the updated item
    var updatedItem = (await repository.ListAsync())
        .FirstOrDefault(project => project.Name == newName);

    Assert.NotNull(updatedItem);
    Assert.NotEqual(category.Name, updatedItem?.Name);
    Assert.Equal(category.Image, updatedItem?.Image);
    Assert.Equal(newCategory.Id, updatedItem?.Id);
  }
}
