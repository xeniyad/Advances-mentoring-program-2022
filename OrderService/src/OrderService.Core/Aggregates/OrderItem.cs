namespace OrderService.Core.Aggregates;

public class OrderItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
    public int Quantity { get; set; }
    public Guid OrderId { get; set; }
}
