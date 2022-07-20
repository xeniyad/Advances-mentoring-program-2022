using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Carting.Tests
{
    [TestClass]
    public class CartingApiTest : IDisposable
    {
        private readonly Mock<ICartingRepository> _cartingRepoMock = new();

        private WebApplicationFactory<Carting.API.Program> _factory;

        public CartingApiTest()
        {
            _factory = new WebApplicationFactory<Carting.API.Program>()
                                  .WithWebHostBuilder(builder =>
                                  {
                                      builder.ConfigureServices(services =>
                                      {
                                          services.AddSingleton(_cartingRepoMock.Object);
                                      });
                                  });

        }


        [TestMethod]
        public async Task GetCart_HappyPath()
        {
            var client = _factory.CreateClient();
            var cartId = new Guid();
            var item = new Item { Name = "Butter", Id = 2, Price = new Money(20.51, Currency.USD), Quantity = 4, Image = null };
            var cart = new Cart() { Id = cartId, Items = new List<Item> { item } };

            _cartingRepoMock.Setup(repo => repo.AddItemToCartAsync(cartId, item)).
              ReturnsAsync(item);

            var response = await client.GetAsync($"cart/{cartId}");
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task GetCart_BadPath()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"cart/123");

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        public void Dispose()
        {
            _factory.Dispose();
        }
    }
}
