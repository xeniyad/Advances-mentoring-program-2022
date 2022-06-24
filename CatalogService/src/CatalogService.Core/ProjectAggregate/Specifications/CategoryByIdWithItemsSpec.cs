using Ardalis.Specification;
using CatalogService.Core.ProjectAggregate;

namespace CatalogService.Core.ProjectAggregate.Specifications;

public class CategoryByIdWithItemsSpec : Specification<Category>, ISingleResultSpecification
{
  public CategoryByIdWithItemsSpec(int categoryId)
  {
    Query
        .Where(category => category.Id == categoryId).
        Include(category => category.Items);
  }
}
