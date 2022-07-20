using Microsoft.EntityFrameworkCore;
using Moq;
using System.Web.Http;

namespace Carting.Tests
{
    [TestClass]
    public class CartingServiceTest
    {
        public CartingServiceTest()
        {
            var configuration = new HttpConfiguration();
            var request = new HttpRequestMessage();
            request.SetConfiguration(configuration);            
        }

        private async Task<Guid> initializeExistingCart(CartingContext context)
        {
            var existingCartId = Guid.NewGuid();

            var cartingRepository = new CartingRepository();
            var logger = new FakeLogger();
            var cartingService = new CartingService(cartingRepository);
            await cartingService.InitializeCartAsync(existingCartId, 
                new Item {
                    Name = "Bread", 
                    Id = 1, 
                    Price = new Money(2.2, Currency.USD), 
                    Quantity = 2, 
                    Image = null 
                });
            await cartingService.AddItemAsync(existingCartId, new Item { Name = "Butter", Id = 2, Price = new Money(20.51, Currency.USD), Quantity = 4, Image = null });

            return existingCartId;
        }

        [TestMethod]
        public async Task InitializeCart_NewId()
        {
            using var factory = new CartingContextConnectionFactory();
            using var context = factory.CreateContext();
            var cartingRepository = new CartingRepository();
            var logger = new FakeLogger();
            var cartingService = new CartingService(cartingRepository);
            var guid = Guid.NewGuid();
            var item = new Item { Name = "Bread", Id = 1, Price = new Money(2.2, Currency.USD), Quantity = 2, Image = null };

            var cartRepo = await cartingService.InitializeCartAsync(guid, item);
            Assert.AreEqual(guid, cartRepo.Id);
            Assert.AreEqual(1, cartRepo.Items.Count);
        }

        [TestMethod]
        public async Task InitializeCart_ExistingId()
        {
            using var factory = new CartingContextConnectionFactory();
            using var context = factory.CreateContext();
            var cartingRepository = new CartingRepository(); 
            var logger = new FakeLogger();
            var cartingService = new CartingService(cartingRepository);

            var existingCartId = await initializeExistingCart(context);

            var cartRepo = await cartingService.InitializeCartAsync(existingCartId);

            Assert.AreEqual(cartRepo.Id, existingCartId);
            Assert.AreEqual(2, cartRepo.Items.Count);
        }

        [TestMethod]
        public async Task GetAllItemsForCart()
        {
            using var factory = new CartingContextConnectionFactory();
            using var context = factory.CreateContext();
            var cartingRepository = new CartingRepository();
            var logger = new FakeLogger();
            var cartingService = new CartingService(cartingRepository);
            var existingCartId = await initializeExistingCart(context);

            var items = await cartingService.GetCartItemsAsync(existingCartId);

            Assert.AreEqual(2, items.Count);
        }
        
        [TestMethod]
        public async Task AddItemToCart_ExistingCart()
        {
            using var factory = new CartingContextConnectionFactory();
            using var context = factory.CreateContext();
            var cartingRepository = new CartingRepository();
            var logger = new FakeLogger();
            var cartingService = new CartingService(cartingRepository);
            var itemToAdd = new Item { Name = "Milk", Id = 3, Price = new Money(5.1, Currency.USD), Quantity = 1, Image = null };

            var existingCartId = await initializeExistingCart(context);
            await cartingService.AddItemAsync(existingCartId, itemToAdd);
            var items = await cartingService.GetCartItemsAsync(existingCartId);

            Assert.AreEqual(3, items.Count);
        }

        [TestMethod]
        public async Task AddItemToCart_ExistingItem_IncreaseQuantity()
        {
            using var factory = new CartingContextConnectionFactory();
            using var context = factory.CreateContext();
            var cartingRepository = new CartingRepository();
            var logger = new FakeLogger();
            var cartingService = new CartingService(cartingRepository);
            var itemToAdd = new Item { Name = "Bread", Id = 1, Price = new Money(2.2, Currency.USD), Quantity = 3, Image = null };

            var existingCartId = await initializeExistingCart(context);
            await cartingService.AddItemAsync(existingCartId, itemToAdd);
            var itemsAfter = await cartingService.GetCartItemsAsync(existingCartId);

            Assert.AreEqual(2, itemsAfter.Count);
            Assert.AreEqual((uint)5, itemsAfter.First(i => i.Id == 1).Quantity);
        }

        [TestMethod]
        public async Task RemoveExistingItemFromExistingCart_CountDecreases()
        {
            using var factory = new CartingContextConnectionFactory();
            using var context = factory.CreateContext();
            var cartingRepository = new CartingRepository();
            var logger = new FakeLogger();
            var itemToRemove = new Item { Name = "Bread", Id = 1, Price = new Money(2.2, Currency.USD), Quantity = 2, Image = null };
            var cartingService = new CartingService(cartingRepository);

            var existingCartId = await initializeExistingCart(context); 
            await cartingService.RemoveItemAsync(existingCartId, itemToRemove.Id);
            var itemsAfter = await cartingService.GetCartItemsAsync(existingCartId);

            Assert.AreEqual(1, itemsAfter.Count);
        }

        [TestMethod]
        public async Task RemoveNonExistingItemFromExistingCart_CountNotChange()
        {
            using var factory = new CartingContextConnectionFactory();
            using var context = factory.CreateContext();
            var cartingRepository = new CartingRepository();
            var logger = new FakeLogger();
            var itemToRemove = new Item { Name = "Milk", Id = 3, Price = new Money(5.1, Currency.USD), Quantity = 1, Image = null };
            var cartingService = new CartingService(cartingRepository);

            var existingCartId = await initializeExistingCart(context); 
            await cartingService.RemoveItemAsync(existingCartId, itemToRemove.Id);
            var itemsAfter = await cartingService.GetCartItemsAsync(existingCartId);

            Assert.AreEqual(2, itemsAfter.Count);
        }

        [TestMethod]
        public async Task GetNonExistingCart_AddWarningToLog()
        {
            using var factory = new CartingContextConnectionFactory();
            using var context = factory.CreateContext();
            var cartingRepository = new CartingRepository();
            var logger = new FakeLogger();
            var guid = Guid.NewGuid();
            var cartingService = new CartingService(cartingRepository);

            var items = await cartingService.GetCartItemsAsync(Guid.NewGuid());

            Assert.AreEqual(1, logger.Events.Where(e => e.Type == EventTypes.Warning).Count());
        }
    }
}