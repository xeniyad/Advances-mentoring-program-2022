using CatalogService.Core.Interfaces;
using CatalogService.Core.ProjectAggregate;
using CatalogService.Infrastructure.Data;
using CatalogService.Infrastructure.Services;
using CatalogService.SharedKernel.Interfaces;
using CatalogService.UnitTests.Core.CategoryAggregate;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CatalogService.UnitTests;

public class DatabaseFixture : IDisposable
{
  public ICategoryService CategoryService;
  public IItemService ItemService;
  private AppDbContext _context;
  public DatabaseFixture()
  {
    var options = new DbContextOptionsBuilder<AppDbContext>()
                      .UseInMemoryDatabase(Guid.NewGuid().ToString())
                      .Options;
    var dispatcher = new Mock<IDomainEventDispatcher>();
    _context = new AppDbContext(options, dispatcher.Object);

    _context.Database.EnsureDeleted();
    _context.Database.EnsureCreated();

    SeedData.PopulateTestData(_context);
    CategoryService = new CategoryService(_context);
    ItemService = new ItemService(_context);

    // initialize data in the test database
  }

  public void Dispose()
  {
    _context.Dispose();
  }
}

public class CategoryServiceTest : IClassFixture<DatabaseFixture>
{
  private readonly DatabaseFixture _dbFixture;

  public CategoryServiceTest(DatabaseFixture fixture)
  {
    _dbFixture = fixture;
  }

  [Fact]
  public async Task DeleteCategory_ReturnsEmpty()
  {
    var categoriesBefore = await _dbFixture.CategoryService.GetAllCategories();
    await _dbFixture.CategoryService.DeleteCategory(1);
    var categoriesAfter = await _dbFixture.CategoryService.GetAllCategories();

    Assert.Equal(categoriesBefore.Count - 1, categoriesAfter.Count);
  }
  [Fact]
  public async Task AddCategory_ReturnsNew()
  {
    var categoriesBefore = await _dbFixture.CategoryService.GetAllCategories();
    var newCategory = new Category("new category");
    var addedCategory = await _dbFixture.CategoryService.AddCategory(newCategory);
    var categoriesAfter = await _dbFixture.CategoryService.GetAllCategories();

    Assert.True(addedCategory.Id > 0);
    Assert.Equal(newCategory.Name, addedCategory.Name);
    Assert.Equal(categoriesBefore.Count + 1, categoriesAfter.Count);
  }
  [Fact]
  public async Task UpdateCategory_ChangesName()
  {
    const string categoryName = "updated name";
    var categories = await _dbFixture.CategoryService.GetAllCategories();
    var categoryToUpdate = categories.First();
    categoryToUpdate.UpdateName(categoryName);
    await _dbFixture.CategoryService.UpdateCategory(categoryToUpdate);
    var categoryAfterUpdate = await _dbFixture.CategoryService.GetCategory(categoryToUpdate.Id);

    Assert.Equal(categoryToUpdate.Id, categoryAfterUpdate.Id);
    Assert.Equal(categoryName, categoryAfterUpdate.Name);
  }
  [Fact]
  public async Task AddItemToCategory_IncreasesCount()
  {
    var categories = await _dbFixture.CategoryService.GetAllCategories();
    var categoryToUpdate = categories.First();
    var itemsBefore = categoryToUpdate?.Items?.Count;
    var item = new Item("new item", "desciption", null, new CatalogService.Core.ValueObjects.Money(1.00, CatalogService.Core.ValueObjects.Currency.USD), 5);
    await _dbFixture.ItemService.AddItem(item, categoryToUpdate.Id);
    var categoryAfterUpdate = await _dbFixture.CategoryService.GetCategory(categoryToUpdate.Id);
    Assert.Equal(itemsBefore + 1, categoryAfterUpdate?.Items?.Count);
  }
}
