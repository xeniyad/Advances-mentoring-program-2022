using System.Linq;
using System.Threading.Tasks;
using CatalogService.Application.Categories.Models;
using CatalogService.Core.Interfaces;
using CatalogService.Core.ProjectAggregate;
using CatalogService.Core.ProjectAggregate.Specifications;
using CatalogService.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CatalogService.Web.Pages.ToDoRazorPage;

public class IndexModel : PageModel
{
  private readonly ICategoryService _service;

  [BindProperty(SupportsGet = true)]
  public int CategoryId { get; set; }

  public CategoryDetail? Category { get; set; }

  public IndexModel(ICategoryService service)
  {
    _service = service;
  }

}
