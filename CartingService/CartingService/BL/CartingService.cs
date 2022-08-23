using Carting.DL;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Carting.BL
{
    public class CartingService
    {
        private readonly ICartingRepository _repository;

        public CartingService(ICartingRepository repository)
        {
            _repository = repository;
        }

        public async Task<Item> AddItemAsync(Guid cartId, Item item)
        {
            var cart = await _repository.GetCartAsync(cartId);
            if (cart == null)
            {
                cart = await InitializeCartAsync(cartId, item);
            }
            var existingItem = cart.Items.Find(i => i.Id == item.Id);
            if (existingItem != null)
            {
                await _repository.UpdateItemQuantityAsync(cartId, existingItem.Id, item.Quantity);
                return item;
            }
            else
            {
                return await _repository.AddItemToCartAsync(cartId, item);
            }
        }

        public async Task<Item> UpdateItemAsync(Guid cartId, int itemId, Money itemPrice, string itemName)
        {
            var cart = await _repository.GetCartAsync(cartId);
            if (cart != null)
            {
                var existingItem = cart.Items.Find(i => i.Id == itemId);
                if (existingItem != null)
                {
                    await _repository.UpdateItemAsync(cartId, itemId, itemPrice, itemName);
                }
                return existingItem;
            }
            return null;
        }
        public async Task<IList<Item>> GetCartItemsAsync(Guid cartId)
        {
            var cart = await _repository.GetCartAsync(cartId);
            if (cart == null)
            {
                return new List<Item>();
            }
            else
                return cart.Items;

        }

        public async Task<IList<Cart>> GetAllCartsAsync()
        {
            var carts = await _repository.GetAllCarts();
            if (carts == null)
            {
                return new List<Cart>();
            }
            else
                return carts;

        }

        public async Task<Cart> GetCartAsync(Guid cartId)
        {
            return await _repository.GetCartAsync(cartId);

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
        public async Task<bool> RemoveItemAsync(Guid cartId, int itemId)
        {
            return await _repository.RemoveItemFromCartAsync(cartId, itemId);
        }
    }
}
