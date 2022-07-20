using CatalogService.Core.Interfaces;
using CatalogService.Core.ProjectAggregate;
using Microsoft.AspNetCore.TestHost;
using CatalogService.Core.ValueObjects;
using Moq;
using Xunit;
using System.Net;
using Newtonsoft.Json;
using Ardalis.GuardClauses;
using Ardalis.Result;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CatalogService.UnitTests;
public class CategoryApiTest
{
  private readonly Mock<ICategoryService> _categoryServiceMock = new();

  private WebApplicationFactory<CatalogService.Web.Program> _factory = null;

  public CategoryApiTest()
  {
    _factory = new WebApplicationFactory<CatalogService.Web.Program>()
                          .WithWebHostBuilder(builder =>
                          {
                            builder.ConfigureServices(services =>
                            {
                              services.AddSingleton(_categoryServiceMock.Object);
                            });
                          });

  }


  [Fact]
  public async Task GetCategory_HappyPath()
  {
    var client = _factory.CreateClient();
    var category1 = new Category(1, "Food", null, null);
    var category2 = new Category("Grocery", 1, null);
    //var item1 = new Item() { Name = "Blouse", Amount = 5, Price = new Money(5, Currency.USD) };
    //category.Items = new List<Item>();
    //category.Items.Add(item1);

    _categoryServiceMock.Setup(service => service.AddCategory("Food", null, null)).
      ReturnsAsync(category1);
    _categoryServiceMock.Setup(service => service.AddCategory(category2)).
      ReturnsAsync(category2);

    var response = await client.GetAsync($"api/v1/category/{category1.Id}");
    response.EnsureSuccessStatusCode();
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
  }

  [Fact]
  public async Task GetCategory_BadPath()
  {
    var client = _factory.CreateClient();
    var category1 = new Category(1, "Food", null, null);
    //var item1 = new Item() { Name = "Blouse", Amount = 5, Price = new Money(5, Currency.USD) };
    //category.Items = new List<Item>();
    //category.Items.Add(item1);

    _categoryServiceMock.Setup(service => service.AddCategory(category1)).
      ReturnsAsync(category1);

    var response = await client.GetAsync($"api/v1/category/100");
    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }

}
