using Ardalis.Result;
using Microsoft.Extensions.Logging;
using OrderService.Core.Aggregates;
using OrderService.Core.Interfaces;

namespace OrderService.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly ICartServiceClient _cartClient;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IOrderRepository repository, ICartServiceClient cartClient, ILogger<OrderService> logger)
    {
        _repository = repository;
        _cartClient = cartClient;
        _logger = logger;
    }

    public async Task<Result<Order>> PlaceOrderAsync(string userId, Guid cartId, CancellationToken cancellationToken = default)
    {
        var cart = await _cartClient.GetCartAsync(cartId, cancellationToken);
        if (cart == null || cart.Items.Count == 0)
            return Result.NotFound($"Cart {cartId} not found or is empty.");

        var order = new Order(userId, cartId);
        foreach (var item in cart.Items)
        {
            order.AddItem(item.Id, item.Name, item.Price, item.Currency, item.Quantity);
        }

        await _repository.AddAsync(order, cancellationToken);
        _logger.LogInformation("Order {OrderId} placed for user {UserId} from cart {CartId}", order.Id, userId, cartId);

        foreach (var item in cart.Items)
        {
            try
            {
                await _cartClient.RemoveItemAsync(cartId, item.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to remove item {ItemId} from cart {CartId} after order placement", item.Id, cartId);
            }
        }

        return Result.Success(order);
    }

    public async Task<Result<IReadOnlyList<Order>>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken = default)
    {
        var orders = await _repository.GetByUserIdAsync(userId, cancellationToken);
        return Result.Success(orders);
    }

    public async Task<Result<Order>> GetOrderByIdAsync(Guid orderId, string userId, CancellationToken cancellationToken = default)
    {
        var order = await _repository.GetByIdAsync(orderId, cancellationToken);
        if (order == null) return Result.NotFound($"Order {orderId} not found.");
        if (order.UserId != userId) return Result.Forbidden();
        return Result.Success(order);
    }
}
