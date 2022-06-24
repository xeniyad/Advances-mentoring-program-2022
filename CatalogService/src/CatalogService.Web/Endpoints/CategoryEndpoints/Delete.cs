using Ardalis.ApiEndpoints;
using CatalogService.Core.ProjectAggregate;
using CatalogService.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CatalogService.Web.Endpoints.ProjectEndpoints;

public class Delete : EndpointBaseAsync
    .WithRequest<DeleteProjectRequest>
    .WithoutResult
{
  private readonly IRepository<Category> _repository;

  public Delete(IRepository<Category> repository)
  {
    _repository = repository;
  }

  [HttpDelete(DeleteProjectRequest.Route)]
  [SwaggerOperation(
      Summary = "Deletes a Project",
      Description = "Deletes a Project",
      OperationId = "Projects.Delete",
      Tags = new[] { "ProjectEndpoints" })
  ]
  public override async Task<ActionResult> HandleAsync([FromRoute] DeleteProjectRequest request,
      CancellationToken cancellationToken)
  {
    var aggregateToDelete = await _repository.GetByIdAsync(request.ProjectId); // TODO: pass cancellation token
    if (aggregateToDelete == null) return NotFound();

    await _repository.DeleteAsync(aggregateToDelete);

    return NoContent();
  }
}
