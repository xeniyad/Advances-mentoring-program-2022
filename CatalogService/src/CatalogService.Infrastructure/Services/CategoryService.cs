using Ardalis.Result;
using CatalogService.Core.Interfaces;
using CatalogService.Core.ProjectAggregate;
using CatalogService.Core.ProjectAggregate.Specifications;
using CatalogService.SharedKernel.Interfaces;

namespace CatalogService.Infrastructure.Services;
public class CategoryService : ICategoryService
{
  private readonly IRepository<Category> _repository;

  public CategoryService(IRepository<Category> repository)
  {
    _repository = repository;
  }
  public async Task AddCategory(string name, int? parentId, string? image)
  {
    var category = new Category(name, parentId, image);
    await _repository.AddAsync(category);
    await _repository.SaveChangesAsync();
  }

  public async Task<Category> AddCategory(Category category)
  {    
    var newCategory = await _repository.AddAsync(category);
    await _repository.SaveChangesAsync();
    return newCategory;
  }

  public async Task AddItemToCategory(int categoryId, Item item)
  {    
    var category = await _repository.GetByIdAsync(categoryId);
    if (category != null)
    {
      if (category.Items == null) { category.Items = new List<Item>(); }
      category.Items.Add(item);
      await _repository.UpdateAsync(category);
      await _repository.SaveChangesAsync();
    }
  }

  public async Task DeleteCategory(int categoryId)
  {
    var category = await _repository.GetByIdAsync(categoryId);
    if (category != null)
    {
      await _repository.DeleteAsync(category);
      await _repository.SaveChangesAsync();
    }
  }

  public async Task<List<Category>> GetAllCategories()
  {
    return await _repository.ListAsync();
  }

  public async Task<Category> GetCategory(int categoryId)
  {
    var category = await _repository.GetByIdAsync(categoryId);

    if (category == null) return Result<Category>.NotFound();

    return new Result<Category>(category);
  }

  public async Task<Category> GetCategoryWithItems(int categoryId)
  {
    var spec = new CategoryByIdWithItemsSpec(categoryId);
    var category = await _repository.GetBySpecAsync(spec);

    // TODO: Optionally use Ardalis.GuardClauses Guard.Against.NotFound and catch
    if (category?.Items == null) return Result<Category>.NotFound();

    return new Result<Category>(category);
  }

  public async Task UpdateCategory(Category category)
  {
    var existCategory = await _repository.GetByIdAsync(category.Id);
    if (existCategory != null)
    {
      await _repository.UpdateAsync(category);
      await _repository.SaveChangesAsync();
    } 
  }
}
