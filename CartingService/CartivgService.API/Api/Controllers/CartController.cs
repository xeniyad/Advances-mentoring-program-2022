using Carting.BL;
using Carting.BL.DTO;
using Carting.DL;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Carting.API.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    public class Cart1Controller : ControllerBase
    {
        private readonly CartingService _service;
        public Cart1Controller(ICartingRepository repository)
        {
            _service = new CartingService(repository);
        }

        [HttpGet]
        [Route("cart/{cartId}")]
        [ProducesResponseType(typeof(CartDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetCartInfo(string cartId)
        {
            var cart = await _service.GetCartAsync(new Guid(cartId));
            if (cart == null) return NotFound();
            var response = new CartDTO()
            {
                Id = cart.Id.ToString(),
                Items = cart.Items.Select(item => new ItemDTO()
                {
                    Id = item.Id,
                    Image = item.Image,
                    Name = item.Name,
                    Price = item.Price,
                    Quantity = item.Quantity
                }).ToList()
            };
            return Ok(response);
        }

        [HttpPost]
        [Route("cart/{cartId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ItemDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> InsertItem(string cartId, [FromBody] ItemDTO item)
        {
            var itemDb = new Item() { 
                Id = item.Id, 
                Image = item.Image, 
                Name = item.Name, 
                Price = item.Price, 
                Quantity = item.Quantity 
            };
            var createdItem = await _service.AddItemAsync(new Guid(cartId), itemDb);
            if (createdItem.Id != default)
            {
                var response = new ItemDTO() { 
                    Id = createdItem.Id,
                    Name = createdItem.Name,
                    Image = createdItem.Image,
                    Price = createdItem.Price,
                    Quantity = createdItem.Quantity
                };
                return Ok(response);
            } else return BadRequest();
        }

        [Route("cart/{cartId}/items/{itemId:int}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<ActionResult> DeleteItem(string cartId, int itemId)
        {
            var result = await _service.RemoveItemAsync(new Guid(cartId), itemId);

            if (result)
                return Ok();
            else
                return NotFound();
        }
    }

    [ApiController]
    [ApiVersion("2.0")]
    public class Cart2Controller : ControllerBase
    {

        private readonly CartingService _service;
        public Cart2Controller(ICartingRepository repository)
        {
            _service = new CartingService(repository);
        }

        [HttpGet]
        [Route("cart/{cartId}")]
        [ProducesResponseType(typeof(Cart), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Cart), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetCartInfo(string cartId)
        {
            var items = await _service.GetCartItemsAsync(new Guid(cartId));
            if (items == null) return NotFound();
            var response = items.Select(item => new ItemDTO()
            {
                Id = item.Id,
                Image = item.Image,
                Name = item.Name,
                Price = item.Price,
                Quantity = item.Quantity
            }).ToList();
            return Ok(response);
        }

        [HttpPost]
        [Route("cart/{cartId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Item))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> InsertItem(string cartId, [FromBody] ItemDTO item)
        {
            var itemDb = new Item()
            {
                Id = item.Id,
                Image = item.Image,
                Name = item.Name,
                Price = item.Price,
                Quantity = item.Quantity
            };
            var createdItem = await _service.AddItemAsync(new Guid(cartId), itemDb);
            if (createdItem.Id != default)
            {
                return Ok(createdItem);
            }
            else return BadRequest();
        }

        [HttpDelete("cart/{cartId}/items/{itemId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<ActionResult> DeleteItem(string cartId, int itemId)
        {
            var result = await _service.RemoveItemAsync(new Guid(cartId), itemId);

            if (result)
                return Ok();
            else
                return NotFound();
        }
    }
}
