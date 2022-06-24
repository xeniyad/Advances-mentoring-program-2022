using Ardalis.HttpClientTestExtensions;
using CatalogService.Web;
using CatalogService.Web.ApiModels;
using Xunit;

namespace CatalogService.FunctionalTests.ControllerApis;

[Collection("Sequential")]
public class ProjectCreate : IClassFixture<CustomWebApplicationFactory<WebMarker>>
{
  private readonly HttpClient _client;

  public ProjectCreate(CustomWebApplicationFactory<WebMarker> factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task ReturnsOneProject()
  {
    var result = await _client.GetAndDeserialize<IEnumerable<CategoryDTO>>("/api/projects");

    Assert.Single(result);
    Assert.Contains(result, i => i.Name == SeedData.TestCategory1.Name);
  }
}
