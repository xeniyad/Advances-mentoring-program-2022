using Carting.DL;
using LiteDB;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Carting.DL
{
    public class CartingRepository : ICartingRepository
    {
        private readonly CartingContext _cartingContext;
        public CartingRepository(CartingContext context)
        {
            _cartingContext = context;
        }

        public bool EnsureDbCreated()
        {

            return _cartingContext.Database.EnsureCreated();

        }

        public bool IsHasData()
        {

            return _cartingContext.Carts.Any();

        }

        public async Task<Item> AddItemToCartAsync(Guid id, Item item)
        {

            var cart = await _cartingContext.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id);
            if (cart == null)
            {
                cart = await CreateCartAsync(id);
            }
            else
            {
                var existingItem = cart.Items.Find(i => i.Id == item.Id);
                if (existingItem != null)
                {
                    existingItem.Quantity += item.Quantity;
                }
                else
                {
                    item.Cart = cart;
                    _cartingContext.Items.Add(item);
                }
                try
                {
                    await _cartingContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.CurrentValues);
                }
            }
            return item;

        }
        public async Task<Cart> CreateCartAsync(Guid id)
        {

            var cart = await GetCartAsync(id);
            if (cart != null)
                return cart;

            var newCart = new Cart() { Id = id, Items = new List<Item>() };
            _cartingContext.Carts.Add(newCart);

            try
            {
                await _cartingContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                entry.OriginalValues.SetValues(entry.CurrentValues);
            }
            return newCart;

        }
        public async Task<Cart> GetCartAsync(Guid id)
        {

            var result = await _cartingContext.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id);
            return result;

        }
        public async Task<bool> RemoveItemFromCartAsync(Guid id, int itemId)
        {

            var cart = await GetCartAsync(id);
            if (cart == null)
                return false;
            var item = cart.Items.First(i => i.Id == itemId);

            if (item != null)
                _cartingContext.Items.Remove(item);

            try
            {
                await _cartingContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                entry.OriginalValues.SetValues(entry.CurrentValues);
                return false;
            }
            return true;

        }
        public async Task UpdateItemQuantityAsync(Guid id, int itemId, uint quantity)
        {

            var cart = await GetCartAsync(id);
            if (cart == null)
                return;

            var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                return;
            item.Quantity += quantity;
            _cartingContext.Update(item);

            try
            {
                await _cartingContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                entry.OriginalValues.SetValues(entry.CurrentValues);
            }

        }

        public async Task<IList<Cart>> GetAllCarts()
        {

            var result = await _cartingContext.Carts.Include(c => c.Items).ToListAsync();
            return result;

        }

        public async Task UpdateItemAsync(Guid id, int itemId, Money price, string name)
        {
            var cart = await GetCartAsync(id);
            if (cart == null)
                return;

            var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                return;
            item.Price = price;
            item.Name = name;
            _cartingContext.Update(item);

            try
            {
                await _cartingContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                entry.OriginalValues.SetValues(entry.CurrentValues);
            }
        }
    }
}
