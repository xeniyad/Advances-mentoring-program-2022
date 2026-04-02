using System.Text.Json;
using Microsoft.Extensions.Logging;
using OrderService.Core.Interfaces;

namespace OrderService.Infrastructure.Services;

public class CartServiceClient : ICartServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CartServiceClient> _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    public CartServiceClient(HttpClient httpClient, ILogger<CartServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<CartDto?> GetCartAsync(Guid cartId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"cart/{cartId}", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("CartingService returned {Status} for cart {CartId}", response.StatusCode, cartId);
                return null;
            }
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var cartDto = JsonSerializer.Deserialize<CartServiceResponse>(content, _jsonOptions);
            if (cartDto == null) return null;

            var items = cartDto.Items.Select(i => new CartItemDto(
                i.Id,
                i.Name ?? string.Empty,
                i.Price?.Amount ?? 0,
                i.Price?.Currency ?? "USD",
                i.Quantity)).ToList();

            return new CartDto(cartDto.Id, items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching cart {CartId} from CartingService", cartId);
            return null;
        }
    }

    public async Task RemoveItemAsync(Guid cartId, int itemId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"cart/{cartId}/items/{itemId}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Failed to delete item {ItemId} from cart {CartId}: {Status}", itemId, cartId, response.StatusCode);
        }
    }

    private record CartServiceResponse(Guid Id, List<CartItemResponse> Items);
    private record CartItemResponse(int Id, string? Name, CartPriceResponse? Price, int Quantity);
    private record CartPriceResponse(decimal Amount, string Currency);
}
