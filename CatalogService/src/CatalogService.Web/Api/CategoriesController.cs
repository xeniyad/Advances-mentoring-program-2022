using CatalogService.Core.Interfaces;
using CatalogService.Core.ProjectAggregate;
using CatalogService.Web.Models.Categories;
using CatalogService.Application.Categories.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace CatalogService.Web.Api;

[Route("api/v1/category")]
[ApiController]
[Produces("application/json", "application/xml")]
[Consumes("application/json", "application/xml")]
public class CategoriesController : BaseApiController
{
  private readonly ICategoryService _categoryService;
  private readonly IItemService _itemService;
  private readonly CategoryResourceFactory _resourceFactory;

  public CategoriesController(ICategoryService categoryService, IItemService itemService, CategoryResourceFactory resourceFactory)
  {
    _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
    _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
    _resourceFactory = resourceFactory ?? throw new ArgumentNullException(nameof(resourceFactory));
  }

  [AllowAnonymous]
  [HttpOptions(Name = nameof(GetCategoryOptions))]
  public IActionResult GetCategoryOptions()
  {
    Response.Headers.Add("Allow", "GET,OPTIONS,POST,PUT,DELETE");

    return Ok();
  }

  [AllowAnonymous]
  [HttpGet(Name = nameof(GetCategoriesList))]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
  [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
  [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  [ResponseCache(CacheProfileName = "Default10")]
  public async Task<IActionResult> GetCategoriesList()
  {
    var categories = await _categoryService.GetAllCategories();

    return Ok(_resourceFactory.CreateCategoryResourceList(categories));
  }

  [AllowAnonymous]
  [Route("{categoryId}")]
  [HttpGet("{categoryId:int}", Name = nameof(GetCategoryById))]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
  [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> GetCategoryById([FromRoute] int categoryId)
  {
    var category = await _categoryService.GetCategory(categoryId);

    if (category == null) return NotFound();

    return Ok(_resourceFactory.CreateCategoryResource(category));
  }

  [EnableCors]
  [Authorize(Roles = "catalog/create")]
  [HttpPost(Name = nameof(CreateCategory))]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
  [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
  [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> CreateCategory([FromBody] CategoryForCreate category)
  {
    var newCategory = new Category(category.Name, category.ParentId, category.Image?.ToString());

    var createdCategory = await _categoryService.AddCategory(newCategory);

    return CreatedAtAction(
        actionName: nameof(GetCategoryById),
        routeValues: new { categoryId = createdCategory.Id },
        value: _resourceFactory.CreateCategoryResource(createdCategory));
  }

  [Authorize(Roles = "catalog/update")]
  [HttpPut(Name = nameof(UpdateCategory))]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
  [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
  [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> UpdateCategory([FromBody] CategoryForUpdate category)
  {
    var updatedCategory = new Category(
      category.Id,
      category.Name,
      category.ParentId,
      category.Image?.Url);

    await _categoryService.UpdateCategory(updatedCategory);

    return Ok();
  }

  [Authorize(Roles = "catalog/delete")]
  [HttpDelete("{categoryId:int}", Name = nameof(DeleteCategory))]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
  [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
  [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> DeleteCategory(int categoryId)
  {
    await _itemService.DeleteCategoryItems(categoryId);
    await _categoryService.DeleteCategory(categoryId);

    return Ok();
  }
}
