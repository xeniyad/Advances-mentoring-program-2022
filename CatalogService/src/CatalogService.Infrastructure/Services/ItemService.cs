using CatalogService.Core.Interfaces;
using CatalogService.Core.ProjectAggregate;
using CatalogService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Services;
public class ItemService : IItemService
{
  private readonly AppDbContext _context;

  public ItemService(AppDbContext context)
  {
    _context = context;
  }
  public async Task<Item> GetItem(int itemId)
  {
    return await _context.Items.FindAsync(itemId);
  }

  public async Task UpdateItem(Item item, int categoryId)
  {
    var category = await _context.Categories.Include(c => c.Items).FirstAsync(c => c.Id == categoryId);
    var existingItem = category?.Items?.FirstOrDefault(i => i.Id == item.Id);
    if (existingItem != null)
    {
      existingItem.Name = item.Name;
      existingItem.Price = item.Price;
      existingItem.Image = item.Image;
      existingItem.Amount = item.Amount;
    }
    await _context.SaveChangesAsync();
  }
  public async Task DeleteItem(int itemId)
  {
    var item = await _context.Items.FindAsync(itemId);
    _context.Items.Remove(item);
    await _context.SaveChangesAsync();
  }

  public async Task DeleteCategoryItems(int categoryId)
  {
    var category = await _context.Categories.FindAsync(categoryId);
    category?.Items?.Clear();
    await _context.SaveChangesAsync();
  }
  public async Task<Item> AddItem(Item item, int categoryId)
  {
    var category = await _context.Categories.FindAsync(categoryId);
    if (category.Items == null)
    {
      category.Items = new List<Item>();
    }
    category.Items.Add(item);
    await _context.SaveChangesAsync();
    return item;
  }

  public async Task<List<Item>> GetPageItems(int categoryId, int pageSize, int pageIndex)
  {
    return await _context.Items
            .Where(i => i.Category.Id == categoryId)
            .OrderBy(i => i.Name)
            .Skip(pageSize * (pageIndex-1))
            .Take(pageSize)
            .ToListAsync();
  }

  public async Task<int> GetItemsTotalAmount(int categoryId)
  {
    return await _context.Items
            .Where(i => i.Category.Id == categoryId)
            .CountAsync();
  }
}
