using CatalogService.Core.Interfaces;
using CatalogService.Core.ProjectAggregate;
using CatalogService.Core.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using CatalogService.Web.Models.Items;
using CatalogService.Application.Items.Models;
using CatalogService.Web.Integration;
using CatalogService.Application.IntegrationEvents;
using Microsoft.AspNetCore.Authorization;

namespace CatalogService.Web.Api;

[ApiController]
[Route("api/v1/category/{categoryId:int}/items")]
[Produces("application/json", "application/xml")]
[Consumes("application/json", "application/xml")]
public class ItemsController : BaseApiController
{
  private readonly IItemService _itemService;
  private readonly ItemResourceFactory _resourceFactory;
  private readonly ICatalogIntegrationEventService _intergrationService;

  public ItemsController(IItemService itemService, ItemResourceFactory resourceFactory, ICatalogIntegrationEventService catalogIntegrationEventService)
  {
    _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
    _resourceFactory = resourceFactory ?? throw new ArgumentNullException(nameof(resourceFactory));
    _intergrationService = catalogIntegrationEventService ?? throw new ArgumentNullException(nameof(catalogIntegrationEventService)); 
  }

  [HttpOptions(Name = nameof(GetItemOptions))]
  public IActionResult GetItemOptions()
  {
    Response.Headers.Add("Allow", "GET,OPTIONS,POST,PUT,DELETE");

    return Ok();
  }

  [HttpGet(Name = nameof(GetItemsList))]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  [ResponseCache(CacheProfileName = "Default10")]
  public async Task<IActionResult> GetItemsList([FromRoute] int categoryId, [FromQuery] ItemQuery itemQuery)
  {
    var items = await _itemService.GetPageItems(categoryId, itemQuery.Limit, itemQuery.Page);
    var itemCount = await _itemService.GetItemsTotalAmount(categoryId);
    var pagedItems = new PagedCollection<Item>(items, itemCount, itemQuery.Page, itemQuery.Limit);

    return Ok(_resourceFactory.CreateItemResourceList(pagedItems, itemQuery, categoryId));
  }

  [HttpGet("{itemId:int}", Name = nameof(GetItemById))]
  [Route("{itemId:int}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> GetItemById([FromRoute] int categoryId, [FromRoute] int itemId)
  {
    var item = await _itemService.GetItem(itemId);

    if (item == null) return NotFound();

    return Ok(_resourceFactory.CreateItemResource(item, categoryId));
  }

  [Authorize(Roles = "catalog/create")]
  [HttpPost(Name = nameof(CreateItem))]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> CreateItem([FromRoute] int categoryId, [FromBody] ItemForCreate item)
  {
    var newItem = new Item() {
      Name = item.Name,
      Description = item.Description,
      Image = item.Image?.ToString(),
      Amount = item.Amount,
      Price = item.Price
    };

    var createdItem = await _itemService.AddItem(newItem, categoryId);

    return CreatedAtAction(
        actionName: nameof(GetItemById),
        routeValues: new { categoryId = createdItem.Id },
        value: _resourceFactory.CreateItemResource(createdItem, categoryId));
  }

  [Authorize(Roles = "catalog/update")]
  [HttpPut(Name = nameof(UpdateItem))]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> UpdateItem([FromRoute] int categoryId, [FromBody] ItemForUpdate item)
  {
    if (!User.IsInRole("catalog/update")) return Unauthorized();
    var oldItem = await _itemService.GetItem(item.Id);
    var dbItem = new Item()
    {
      Id = item.Id,
      Name = item.Name,
      Description = item.Description,
      Image = item.Image?.Url,
      Amount = item.Amount,
      Price = item.Price
    };

    await _itemService.UpdateItem(dbItem, categoryId);

    if (oldItem != null && item != null && (oldItem.Price != item.Price || oldItem.Name != item.Name))
    {
      var itemChangedEvent = new ItemChangedIntegrationEvent(item.Id, item.Price, item.Name);   

      // Publish through the Event Bus and mark the saved event as published
      await _intergrationService.PublishThroughEventBusAsync(itemChangedEvent);
    }

    return Ok();
  }

  [Authorize(Roles = "catalog/delete")]
  [HttpDelete("{itemId:int}", Name = nameof(DeleteItem))]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> DeleteItem(int itemId)
  {
    await _itemService.DeleteItem(itemId);

    return Ok();
  }
}
