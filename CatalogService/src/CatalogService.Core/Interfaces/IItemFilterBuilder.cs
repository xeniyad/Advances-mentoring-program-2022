using System.Linq.Expressions;
using CatalogService.Core.ProjectAggregate;

namespace CatalogService.Core.Interfaces;
public interface IItemFilterBuilder
{
  Expression<Func<Item, bool>> Filter { get; }

  IItemFilterBuilder WherePrice(double? minimumPrice, double? maximumPrice);
}
