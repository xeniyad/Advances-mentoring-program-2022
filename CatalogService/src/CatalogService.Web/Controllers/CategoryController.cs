using CatalogService.Core.Interfaces;
using CatalogService.Core.ProjectAggregate;
using CatalogService.Core.ProjectAggregate.Specifications;
using CatalogService.SharedKernel.Interfaces;
using CatalogService.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Web.Controllers;

[Route("[controller]")]
public class CategoryController : Controller
{
  private readonly ICategoryService _categoryService;

  public CategoryController(ICategoryService categoryService)
  {
    _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
  }

  // GET project/{projectId?}
  [HttpGet("{categoryId:int}")]
  public async Task<IActionResult> Index(int categoryId = 1)
  {
    var category = await _categoryService.GetCategoryWithItems(categoryId);
    if (category == null)
    {
      return NotFound();
    }

    var dto = new CategoryViewModel
    {
      Id = category.Id,
      Name = category.Name,
      Items = category.Items?
                    .Select(item => ItemViewModel.FromItem(item))
                    .ToList() ?? new List<ItemViewModel>()
    };
    return View(dto);
  }
}
