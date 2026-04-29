using CatalogService.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CatalogService.Web.Api;

[Route("api/v1/images")]
[ApiController]
[Authorize]
public class ImagesController : ControllerBase
{
  private readonly IBlobStorageService _blob;

  public ImagesController(IBlobStorageService blob) => _blob = blob;

  [HttpPost]
  [Consumes("multipart/form-data")]
  public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
  {
    if (file is null || file.Length == 0)
      return BadRequest("No file provided.");

    var allowed = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
    if (!allowed.Contains(file.ContentType))
      return BadRequest("Unsupported image type.");

    await using var stream = file.OpenReadStream();
    var url = await _blob.UploadAsync(stream, file.FileName, file.ContentType, ct);
    return Ok(new { url });
  }
}

