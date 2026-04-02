using OrderService.Core.Aggregates;

namespace OrderService.Core.Interfaces;

public record CartItemDto(int Id, string Name, decimal Price, string Currency, int Quantity);
public record CartDto(Guid Id, IReadOnlyList<CartItemDto> Items);

public interface ICartServiceClient
{
    Task<CartDto?> GetCartAsync(Guid cartId, CancellationToken cancellationToken = default);
    Task RemoveItemAsync(Guid cartId, int itemId, CancellationToken cancellationToken = default);
}
