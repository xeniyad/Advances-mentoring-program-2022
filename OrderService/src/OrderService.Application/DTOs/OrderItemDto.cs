namespace OrderService.Application.DTOs;

public record OrderItemDto(int ProductId, string Name, decimal Price, string Currency, int Quantity);
