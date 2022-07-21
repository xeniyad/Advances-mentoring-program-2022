using Ardalis.Result;
using CatalogService.Core.Interfaces;
using CatalogService.Core.ProjectAggregate;
using CatalogService.Core.ProjectAggregate.Specifications;
using CatalogService.Infrastructure.Data;
using CatalogService.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Services;
public class CategoryService : ICategoryService
{
  private readonly AppDbContext _context;

  public CategoryService(AppDbContext context)
  {
    _context = context;
  }
  public async Task<Category> AddCategory(string name, int? parentId, string? image)
  {
    var category = new Category(name, parentId, image);
    var createdCategory = await _context.AddAsync(category);
    await _context.SaveChangesAsync();
    return category;
  }

  public async Task<Category> AddCategory(Category category)
  {    
    await _context.Categories.AddAsync(category);
    await _context.SaveChangesAsync();
    return category;
  }

  public async Task AddItemToCategory(int categoryId, Item item)
  {    
    var category = await _context.Categories.FindAsync(categoryId);
    if (category != null)
    {
      if (category.Items == null) { category.Items = new List<Item>(); }
      category.Items.Add(item);
      await _context.SaveChangesAsync();
    }
  }

  public async Task DeleteCategory(int categoryId)
  {
    var category = await _context.Categories.FindAsync(categoryId);
    if (category != null)
    {
      _context.Categories.Remove(category);
      await _context.SaveChangesAsync();
    }
  }

  public async Task<List<Category>> GetAllCategories()
  {
    return await _context.Categories.ToListAsync();
  }



  public async Task<Category> GetCategory(int categoryId)
  {
    var category = await _context.Categories.FindAsync(categoryId);

    if (category == null) return Result<Category>.NotFound();

    return new Result<Category>(category);
  }
   

  public async Task<Category> GetCategoryWithAllItems(int categoryId)
  {
    var category = await _context.Categories.Include(c => c.Items).FirstAsync(c => c.Id == categoryId);

    // TODO: Optionally use Ardalis.GuardClauses Guard.Against.NotFound and catch
    if (category?.Items == null) return Result<Category>.NotFound();

    return new Result<Category>(category);
  }


  public async Task UpdateCategory(Category category)
  {
    var existCategory = await _context.Categories.FindAsync(category.Id);
    if (existCategory != null)
    {
      existCategory.UpdateName(category.Name);
      existCategory.ParentId = category.ParentId;
      existCategory.Image = category.Image;
      await _context.SaveChangesAsync();
    } 
  }
}
