namespace CatalogService.Web.Endpoints.ProjectEndpoints;

public class UpdateProjectResponse
{
  public UpdateProjectResponse(CategoryRecord project)
  {
    Project = project;
  }
  public CategoryRecord Project { get; set; }
}
