using Ardalis.Result;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs;
using OrderService.Core.Interfaces;

namespace OrderService.Web.Api.Controllers;

[ApiController]
[Route("orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyOrders(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var result = await _orderService.GetUserOrdersAsync(userId, cancellationToken);
        return Ok(result.Value.Select(MapToDto));
    }

    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetOrder(Guid orderId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var result = await _orderService.GetOrderByIdAsync(orderId, userId, cancellationToken);
        if (result.Status == ResultStatus.NotFound) return NotFound();
        if (result.Status == ResultStatus.Forbidden) return Forbid();
        return Ok(MapToDto(result.Value));
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromQuery] Guid cartId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var result = await _orderService.PlaceOrderAsync(userId, cartId, cancellationToken);
        if (result.Status == ResultStatus.NotFound) return NotFound(result.Errors);
        return CreatedAtAction(nameof(GetOrder), new { orderId = result.Value.Id }, MapToDto(result.Value));
    }

    private string? GetUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? User.FindFirst("sub")?.Value
        ?? User.FindFirst("oid")?.Value;

    private static OrderDto MapToDto(Core.Aggregates.Order o) => new(
        o.Id, o.UserId, o.CartId, o.Status.ToString(), o.CreatedAt, o.TotalAmount,
        o.Items.Select(i => new OrderItemDto(i.ProductId, i.Name, i.Price, i.Currency, i.Quantity)).ToList());
}
