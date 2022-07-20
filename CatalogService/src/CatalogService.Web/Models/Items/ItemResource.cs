using System.Runtime.Serialization;
using CatalogService.Core.ProjectAggregate;
using CatalogService.Core.ValueObjects;
using CatalogService.Application.Categories.Models;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Web.Models.Items
{
  [DataContract(Name = "Item", Namespace = "")]
  [KnownType(typeof(ResourceBase))]
  public sealed class ItemResource : ResourceBase
  {
    public ItemResource(IUrlHelper urlHelper) : base(urlHelper)
    {
    }

    public ItemResource(IUrlHelper urlHelper, Item item) : base(urlHelper)
    {
      if (item is null)
        throw new ArgumentNullException(nameof(item));

      Id = item.Id;
      Name = item.Name;
      Description = item.Description;
      Image = new Image(item.Image);
      Price = item.Price;
      Amount = item.Amount;
    }

    [DataMember(Order = 1)]
    public int Id { get; set; }

    [DataMember(Order = 2)]
    public string Name { get; set; }

    [DataMember(Order = 3)]
    public string? Description { get; set; } = null;

    [DataMember(Order = 4)]
    public Image? Image { get; set; } = null;

    [DataMember(Order = 5)]
    public Money Price { get; set; }

    [DataMember(Order = 6)]
    public uint Amount { get; set; }
  }
}
