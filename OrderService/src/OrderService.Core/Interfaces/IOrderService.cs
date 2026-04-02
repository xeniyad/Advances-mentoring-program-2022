using Ardalis.Result;
using OrderService.Core.Aggregates;

namespace OrderService.Core.Interfaces;

public interface IOrderService
{
    Task<Result<Order>> PlaceOrderAsync(string userId, Guid cartId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<Order>>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result<Order>> GetOrderByIdAsync(Guid orderId, string userId, CancellationToken cancellationToken = default);
}
