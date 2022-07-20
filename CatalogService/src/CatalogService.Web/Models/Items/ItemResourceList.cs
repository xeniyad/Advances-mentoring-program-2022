using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Web.Models.Items
{
  [DataContract(Name = "ItemsList", Namespace = "")]
  [KnownType(typeof(ResourceBase))]
  public sealed class ItemResourceList : ResourceBase
  {
    public ItemResourceList(IUrlHelper urlHelper, IReadOnlyCollection<ItemResource> items) : base(urlHelper)
    {
      Items = items ?? throw new ArgumentNullException(nameof(items));
    }

    [DataMember(Order = 1)]
    public IEnumerable<ItemResource> Items { get; }
  }
}
