using CatalogService.Core.Interfaces;
using CatalogService.Core.ProjectAggregate;
using CatalogService.Core.ProjectAggregate.Specifications;
using CatalogService.SharedKernel.Interfaces;
using CatalogService.Web.ApiModels;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Web.Api;

/// <summary>
/// A sample API Controller. Consider using API Endpoints (see Endpoints folder) for a more SOLID approach to building APIs
/// https://github.com/ardalis/ApiEndpoints
/// </summary>
public class CategoriesController : BaseApiController
{
  private readonly ICategoryService _categoryService;

  public CategoriesController(ICategoryService categoryService)
  {
    _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
  }

  // GET: api/Categories
  [HttpGet]
  public async Task<IActionResult> List()
  {
    var categories = await _categoryService.GetAllCategories();
    var projectDTOs = categories
        .Select(category => new CategoryDTO
        (
            id: category.Id,
            name: category.Name
        ))
        .ToList();

    return Ok(projectDTOs);
  }


  // POST: api/Categories
  [HttpPost]
  public async Task<IActionResult> Post([FromBody] CreateCategoryDTO request)
  {
    var newCategory = new Category(request.Name, request.ParentId);

    var createdCategory = await _categoryService.AddCategory(newCategory);

    var result = new CategoryDTO
    (
        id: createdCategory.Id,
        name: createdCategory.Name
    );
    return Ok(result);
  }

  // GET: api/Projects
  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetById(int id)
  {
    var category = await _categoryService.GetCategory(id);

    if (category == null) return NotFound();

    var result = new CategoryDTO
    (
        id: category.Id,
        name: category.Name,
        parentId: category.ParentId,        
        image: category.Image
    );

    return Ok(result);
  }

  // Put: api/Categories
  [HttpPut]
  public async Task<IActionResult> Update([FromBody] CategoryDTO request)
  {
    var updatedCategory = new Category(
      request.Id,
      request.Name,
      request.ParentId,
      request.Image?.Url);

    await _categoryService.UpdateCategory(updatedCategory);

    return Ok();
  }

  // Put: api/Categories
  [HttpDelete("{id:int}")]
  public async Task<IActionResult> Delete(int id)
  {
    await _categoryService.DeleteCategory(id);

    return Ok();
  }
}
