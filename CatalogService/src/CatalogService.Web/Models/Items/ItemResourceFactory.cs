using System;
using System.Collections.Generic;
using System.Linq;
using CatalogService.Core.ProjectAggregate;
using CatalogService.Application.Items.Models;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Web.Models.Items;
  public sealed class ItemResourceFactory
  {
    private readonly IUrlHelper _urlHelper;

    public ItemResourceFactory(IUrlHelper urlHelper)
    {
      _urlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
    }

    public ItemResource CreateItemResource(Item item, int categoryId)
    {
      if (item is null)
        throw new ArgumentNullException(nameof(item));

      return (ItemResource)new ItemResource(_urlHelper, item)
          .AddDelete("delete-item", "DeleteItem", new { itemId = item.Id })
          .AddGet("self", "GetItemById", new { itemId = item.Id, categoryId = categoryId })
          .AddGet("items", "GetItemsList", new { categoryId = categoryId, itemQuery = new PagedQueryParams() { Page=1, Limit=10} })
          .AddOptions("GetItemOptions")
          .AddPost("create-item", "CreateItem", new { categoryId = categoryId, item = new ItemForCreate()})
          .AddPut("update-item", "UpdateItem", new { categoryId = categoryId, item = new ItemForUpdate()});
    }

    public ResourceBase CreateItemResourceList(IPagedCollection<Item> items, PagedQueryParams query, int categoryId)
    {      
      var itemResources = items
          .Select(item => CreateItemResource(item, categoryId))
          .ToList();      

      var routeName = $"api/v1/category/{categoryId}/items";

      var resources = new ItemResourceList(_urlHelper, itemResources);
      if (query.Limit > 0) {
        resources.AddGet("FirstItemsPageLink", "GetItemsList", new { categoryId = categoryId, page = 1, limit = query.Limit });
        if (query.Page >= 1)
          resources.AddGet("CurrentItemsPageLink", "GetItemsList", new { categoryId = categoryId, page = query.Page, limit = query.Limit });
        if (items.NextPageNumber.HasValue && items.NextPageNumber > 0)
          resources.AddGet("NextItemsPageLink", "GetItemsList", new { categoryId = categoryId, page = items.NextPageNumber, limit = query.Limit });
        if (items.PreviousPageNumber.HasValue && items.PreviousPageNumber > 0)
          resources.AddGet("PreviousItemPageLink", "GetItemsList", new { categoryId = categoryId, page = items.PreviousPageNumber, limit = query.Limit });
        if (items.LastPageNumber > 1)
          resources.AddGet("LastItemsPageLink", "GetItemsList", new { categoryId = categoryId, page = items.LastPageNumber, limit = query.Limit });
      }
    return resources;
  }
}
