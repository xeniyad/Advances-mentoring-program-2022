using System.Linq;
using System.Threading.Tasks;
using CatalogService.Core.ProjectAggregate;
using CatalogService.Core.ProjectAggregate.Specifications;
using CatalogService.SharedKernel.Interfaces;
using CatalogService.Web.ApiModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CatalogService.Web.Pages.ToDoRazorPage;

public class IndexModel : PageModel
{
  private readonly IRepository<Category> _repository;

  [BindProperty(SupportsGet = true)]
  public int CategoryId { get; set; }

  public CategoryDTO? Category { get; set; }

  public IndexModel(IRepository<Category> repository)
  {
    _repository = repository;
  }

}
