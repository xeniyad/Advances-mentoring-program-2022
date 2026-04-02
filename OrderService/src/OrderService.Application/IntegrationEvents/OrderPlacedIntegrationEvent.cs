namespace OrderService.Application.IntegrationEvents;

public record OrderPlacedIntegrationEvent(Guid OrderId, string UserId, Guid CartId, decimal TotalAmount, DateTime PlacedAt);
