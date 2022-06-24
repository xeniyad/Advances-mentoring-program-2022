using Ardalis.ApiEndpoints;
using CatalogService.Core.ProjectAggregate;
using CatalogService.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CatalogService.Web.Endpoints.ProjectEndpoints;

public class Create : EndpointBaseAsync
    .WithRequest<CreateProjectRequest>
    .WithActionResult<CreateProjectResponse>
{
  private readonly IRepository<Category> _repository;

  public Create(IRepository<Category> repository)
  {
    _repository = repository;
  }

  [HttpPost("/Projects")]
  [SwaggerOperation(
      Summary = "Creates a new Project",
      Description = "Creates a new Project",
      OperationId = "Project.Create",
      Tags = new[] { "ProjectEndpoints" })
  ]
  public override async Task<ActionResult<CreateProjectResponse>> HandleAsync(CreateProjectRequest request,
      CancellationToken cancellationToken)
  {
    if (request.Name == null)
    {
      return BadRequest();
    }

    var newProject = new Category(request.Name);

    var createdItem = await _repository.AddAsync(newProject); // TODO: pass cancellation token

    var response = new CreateProjectResponse
    (
        id: createdItem.Id,
        name: createdItem.Name
    );

    return Ok(response);
  }
}
