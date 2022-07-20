using Ardalis.Result;
using CatalogService.Core.ProjectAggregate;

namespace CatalogService.Core.Interfaces;

public interface IItemService
{
  Task<Item> GetItem(int itemId);
  Task DeleteItem(int itemId);
  Task DeleteCategoryItems(int categoryId);
  Task<Item> AddItem(Item item, int categoryId);
  Task UpdateItem(Item item, int categoryId);
  Task<List<Item>> GetPageItems(int categoryId, int pageSize, int pageIndex);
  Task<int> GetItemsTotalAmount(int categoryId);
}
