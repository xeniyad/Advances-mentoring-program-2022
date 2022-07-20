using System.Runtime.Serialization;
using CatalogService.Core.ProjectAggregate;
using CatalogService.Core.ValueObjects;
using CatalogService.Application.Categories.Models;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Web.Models.Categories
{
  [DataContract(Name = "Category", Namespace = "")]
  [KnownType(typeof(ResourceBase))]
  public sealed class CategoryResource : ResourceBase
  {
    public CategoryResource(IUrlHelper urlHelper) : base(urlHelper)
    {
    }

    public CategoryResource(IUrlHelper urlHelper, Category category) : base(urlHelper)
    {
      if (category is null)
        throw new ArgumentNullException(nameof(category));

      Id = category.Id;
      Name = category.Name;
      ParentId = category.ParentId;
      Image = category.Image;
    }

    [DataMember(Order = 1)]
    public int Id { get; set; }

    [DataMember(Order = 2)]
    public string Name { get; set; }

    [DataMember(Order = 3)]
    public int? ParentId { get; set; } = null;

    [DataMember(Order = 4)]
    public string? Image { get; set; } = null;
  }
}
