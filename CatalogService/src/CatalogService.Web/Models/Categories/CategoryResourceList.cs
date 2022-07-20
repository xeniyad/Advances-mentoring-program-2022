using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Web.Models.Categories
{
  [DataContract(Name = "CategoryList", Namespace = "")]
  [KnownType(typeof(ResourceBase))]
  public sealed class CategoryResourceList : ResourceBase
  {
    public CategoryResourceList(IUrlHelper urlHelper, IReadOnlyCollection<CategoryResource> categories) : base(urlHelper)
    {
      Categories = categories ?? throw new ArgumentNullException(nameof(categories));
    }

    [DataMember(Order = 1)]
    public IEnumerable<CategoryResource> Categories { get; }
  }
}
