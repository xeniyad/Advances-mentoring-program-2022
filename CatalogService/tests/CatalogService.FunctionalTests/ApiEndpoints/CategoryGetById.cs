using Ardalis.HttpClientTestExtensions;
using CatalogService.Web;
using CatalogService.Web.Endpoints.ProjectEndpoints;
using Xunit;

namespace CatalogService.FunctionalTests.ApiEndpoints;

[Collection("Sequential")]
public class CategoryGetById : IClassFixture<CustomWebApplicationFactory<WebMarker>>
{
  private readonly HttpClient _client;

  public CategoryGetById(CustomWebApplicationFactory<WebMarker> factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task ReturnsSeedProjectGivenId1()
  {
    var result = await _client.GetAndDeserialize<GetProjectByIdResponse>(GetProjectByIdRequest.BuildRoute(1));

    Assert.Equal(1, result.Id);
    Assert.Equal(SeedData.TestCategory1.Name, result.Name);
    Assert.Equal(3, result.Items.Count);
  }

  [Fact]
  public async Task ReturnsNotFoundGivenId0()
  {
    string route = GetProjectByIdRequest.BuildRoute(0);
    _ = await _client.GetAndEnsureNotFound(route);
  }
}
