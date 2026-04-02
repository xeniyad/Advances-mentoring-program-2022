using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OrderService.Core.Aggregates;
using OrderService.Core.Interfaces;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Services;

namespace OrderService.UnitTests;

[TestClass]
public class OrderServiceTest
{
    private static OrderDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new OrderDbContext(options);
    }

    [TestMethod]
    public async Task PlaceOrder_WithValidCart_CreatesOrder()
    {
        using var context = CreateContext(nameof(PlaceOrder_WithValidCart_CreatesOrder));
        var repository = new OrderRepository(context);

        var cartId = Guid.NewGuid();
        var cartItems = new List<CartItemDto>
        {
            new CartItemDto(1, "Bread", 2.5m, "USD", 2),
            new CartItemDto(2, "Butter", 5.0m, "USD", 1)
        };
        var cart = new CartDto(cartId, cartItems);

        var cartClientMock = new Mock<ICartServiceClient>();
        cartClientMock.Setup(c => c.GetCartAsync(cartId, default)).ReturnsAsync(cart);
        cartClientMock.Setup(c => c.RemoveItemAsync(It.IsAny<Guid>(), It.IsAny<int>(), default)).Returns(Task.CompletedTask);

        var service = new OrderService.Infrastructure.Services.OrderService(
            repository, cartClientMock.Object, NullLogger<OrderService.Infrastructure.Services.OrderService>.Instance);

        var result = await service.PlaceOrderAsync("user123", cartId);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("user123", result.Value.UserId);
        Assert.AreEqual(2, result.Value.Items.Count);
        Assert.AreEqual(10.0m, result.Value.TotalAmount);
    }

    [TestMethod]
    public async Task PlaceOrder_WithEmptyCart_ReturnsNotFound()
    {
        using var context = CreateContext(nameof(PlaceOrder_WithEmptyCart_ReturnsNotFound));
        var repository = new OrderRepository(context);
        var cartClientMock = new Mock<ICartServiceClient>();
        cartClientMock.Setup(c => c.GetCartAsync(It.IsAny<Guid>(), default)).ReturnsAsync((CartDto?)null);

        var service = new OrderService.Infrastructure.Services.OrderService(
            repository, cartClientMock.Object, NullLogger<OrderService.Infrastructure.Services.OrderService>.Instance);

        var result = await service.PlaceOrderAsync("user123", Guid.NewGuid());

        Assert.AreEqual(ResultStatus.NotFound, result.Status);
    }

    [TestMethod]
    public async Task GetUserOrders_ReturnsOnlyUserOrders()
    {
        using var context = CreateContext(nameof(GetUserOrders_ReturnsOnlyUserOrders));
        var repository = new OrderRepository(context);

        var order1 = new Order("user1", Guid.NewGuid());
        var order2 = new Order("user2", Guid.NewGuid());
        await repository.AddAsync(order1);
        await repository.AddAsync(order2);

        var cartClientMock = new Mock<ICartServiceClient>();
        var service = new OrderService.Infrastructure.Services.OrderService(
            repository, cartClientMock.Object, NullLogger<OrderService.Infrastructure.Services.OrderService>.Instance);

        var result = await service.GetUserOrdersAsync("user1");

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(1, result.Value.Count);
        Assert.AreEqual("user1", result.Value[0].UserId);
    }

    [TestMethod]
    public async Task GetOrderById_OtherUsersOrder_ReturnsForbidden()
    {
        using var context = CreateContext(nameof(GetOrderById_OtherUsersOrder_ReturnsForbidden));
        var repository = new OrderRepository(context);

        var order = new Order("user1", Guid.NewGuid());
        await repository.AddAsync(order);

        var cartClientMock = new Mock<ICartServiceClient>();
        var service = new OrderService.Infrastructure.Services.OrderService(
            repository, cartClientMock.Object, NullLogger<OrderService.Infrastructure.Services.OrderService>.Instance);

        var result = await service.GetOrderByIdAsync(order.Id, "user2");

        Assert.AreEqual(ResultStatus.Forbidden, result.Status);
    }
}
