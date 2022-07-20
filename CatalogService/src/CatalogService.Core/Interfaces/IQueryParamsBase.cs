using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Core.Interfaces;
public interface IQueryParamsBase
{
  public IDictionary<string, object> ToRouteValuesDictionary();
}
