using Carting.DL;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carting.BL
{
    public class CartingService
    {
        private readonly ILogger _logger;
        private readonly ICartingRepository _repository;

        public CartingService(ICartingRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;  
        }

        public async Task AddItemAsync(Guid cartId, Item item)
        {
            var cart = await _repository.GetCartAsync(cartId);
            if (cart == null)
            {
                cart = await InitializeCartAsync(cartId, item);
            }
            else
            {
                var existingItem = cart.Items.Find(i => i.Id == item.Id);
                if (existingItem != null)
                    await _repository.UpdateItemQuantityAsync(cartId, existingItem.Id, item.Quantity);
                else
                    await _repository.AddItemToCartAsync(cartId, item);

            }
        }
        public async Task<IList<Item>> GetCartItemsAsync(Guid cartId)
        {
            var cart = await _repository.GetCartAsync(cartId);
            if (cart == null)
            {
                _logger.LogWarning($"No cart with {cartId} found");
                return new List<Item>();
            }
            else
                return cart.Items;

        }
        public async Task<Cart> InitializeCartAsync(Guid cartId, Item item = null)
        {
            var cart = await _repository.GetCartAsync(cartId);
            if (cart == null)
            {
                cart = await _repository.CreateCartAsync(cartId);
                cart.Items = new List<Item>() { };
            }
            if (item != null)
            {
                await _repository.AddItemToCartAsync(cartId, item);
                cart = await _repository.GetCartAsync(cartId);
            }                

            return cart;
        }
        public async Task RemoveItemAsync(Guid cartId, int itemId)
        {
            await _repository.RemoveItemFromCartAsync(cartId, itemId);
        }
    }
}
