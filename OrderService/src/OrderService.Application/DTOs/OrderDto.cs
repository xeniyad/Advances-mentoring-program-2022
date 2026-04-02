namespace OrderService.Application.DTOs;

public record OrderDto(
    Guid Id,
    string UserId,
    Guid CartId,
    string Status,
    DateTime CreatedAt,
    decimal TotalAmount,
    IReadOnlyList<OrderItemDto> Items);
