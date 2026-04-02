using Ardalis.GuardClauses;

namespace OrderService.Core.Aggregates;

public class Order
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; }
    public Guid CartId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public List<OrderItem> Items { get; private set; } = new();

    private Order() { UserId = string.Empty; }

    public Order(string userId, Guid cartId)
    {
        Id = Guid.NewGuid();
        UserId = Guard.Against.NullOrWhiteSpace(userId, nameof(userId));
        CartId = cartId;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddItem(int productId, string name, decimal price, string currency, int quantity)
    {
        Items.Add(new OrderItem
        {
            ProductId = productId,
            Name = name,
            Price = price,
            Currency = currency,
            Quantity = quantity,
            OrderId = Id
        });
    }

    public decimal TotalAmount => Items.Sum(i => i.Price * i.Quantity);

    public void Complete() => Status = OrderStatus.Completed;
    public void Cancel() => Status = OrderStatus.Cancelled;
}
