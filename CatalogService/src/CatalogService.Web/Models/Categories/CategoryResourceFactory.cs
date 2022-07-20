using System;
using System.Collections.Generic;
using System.Linq;
using CatalogService.Core.ProjectAggregate;
using CatalogService.Core.ProjectAggregate.Queries;
using CatalogService.Application.Categories.Models;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Web.Models.Categories
{
  public sealed class CategoryResourceFactory
  {
    private readonly IUrlHelper _urlHelper;

    public CategoryResourceFactory(IUrlHelper urlHelper)
    {
      _urlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
    }

    public CategoryResource CreateCategoryResource(Category category)
    {
      if (category is null)
        throw new ArgumentNullException(nameof(category));

      return (CategoryResource)new CategoryResource(_urlHelper, category)
          .AddDelete("delete-category", "DeleteCategory", new { categoryId = category.Id })
          .AddGet("self", "GetCategoryById", new { categoryId = category.Id })
          .AddGet("categories", "GetCategoriesList", new PagedQueryParams())
          .AddOptions("GetCategoryOptions")
          .AddPost("create-category", "CreateCategory", new { })
          .AddPut("update-category", "UpdateCategory", new { categoryId = category.Id })
          .AddGet("items", "GetCategoryItemsList", new { categoryId = category.Id });
    }

    public ResourceBase CreateCategoryResourceList(IEnumerable<Category> categories)
    {
      var categoryResources = categories
          .Select(category => CreateCategoryResource(category))
          .ToList();

      var routeName = "GetCategoriesList";

      return new CategoryResourceList(_urlHelper, categoryResources);
    }
  }
}
