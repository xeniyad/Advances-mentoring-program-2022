using CatalogService.Core.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Web.Api;

[ApiController]
[Route("api/v1/enums")]
[Produces("application/json")]
public class EnumsController : BaseApiController
{
  [HttpGet("currencies", Name = nameof(GetCurrencies))]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public IActionResult GetCurrencies()
  {
    var currencies = Enum.GetValues<Currency>()
      .Select(c => new { value = (int)c, name = c.ToString() })
      .ToList();

    return Ok(currencies);
  }
}
