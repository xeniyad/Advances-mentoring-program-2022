using Ardalis.HttpClientTestExtensions;
using CatalogService.Web;
using CatalogService.Web.Endpoints.ProjectEndpoints;
using Xunit;

namespace CatalogService.FunctionalTests.ApiEndpoints;

[Collection("Sequential")]
public class CategoryList : IClassFixture<CustomWebApplicationFactory<WebMarker>>
{
  private readonly HttpClient _client;

  public CategoryList(CustomWebApplicationFactory<WebMarker> factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task ReturnsOneProject()
  {
    var result = await _client.GetAndDeserialize<ProjectListResponse>("/Projects");

    Assert.Single(result.Projects);
    Assert.Contains(result.Projects, i => i.Name == SeedData.TestCategory1.Name);
  }
}
