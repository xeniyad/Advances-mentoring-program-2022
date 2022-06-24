using Ardalis.Result;
using CatalogService.Core.ProjectAggregate;

namespace CatalogService.Core.Interfaces;

public interface ICategoryService
{
  Task<Category> GetCategory(int categoryId);
  Task<List<Category>> GetAllCategories();
  Task<Category> GetCategoryWithItems(int categoryId);
  Task DeleteCategory(int categoryId);
  Task AddCategory(string name, int? parentId, string? image);
  Task<Category> AddCategory(Category category);
  Task UpdateCategory(Category category);
  Task AddItemToCategory(int categoryId, Item item);
}
