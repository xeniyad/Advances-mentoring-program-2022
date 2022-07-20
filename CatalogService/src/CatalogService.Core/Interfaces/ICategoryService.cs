using Ardalis.Result;
using CatalogService.Core.ProjectAggregate;

namespace CatalogService.Core.Interfaces;

public interface ICategoryService
{
  Task<Category> GetCategory(int categoryId);
  Task<Category> GetCategoryWithAllItems(int categoryId);
  Task<List<Category>> GetAllCategories();
  Task DeleteCategory(int categoryId);
  Task<Category> AddCategory(string name, int? parentId, string? image);
  Task<Category> AddCategory(Category category);
  Task UpdateCategory(Category category);
}
