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
    public class CartingRepository :  ICartingRepository
    {
        
        private readonly CartingContext _db;

        public CartingRepository(CartingContext db)
        {
            _db = db;
        }

        public async Task AddItemToCartAsync(Guid id, Item item)
        {
            var cart = await _db.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id);
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
                    cart.Items.Add(item);
                }
                try
                {
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.CurrentValues);
                }
            }
        }
        public async Task<Cart> CreateCartAsync(Guid id)
        {
            var cart = await GetCartAsync(id);
            if (cart != null)
                return cart;

            var newCart = new Cart() { Id = id, Items = new List<Item>() };
            _db.Carts.Add(newCart);

            try
            {
                await _db.SaveChangesAsync();
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
            var result = await _db.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id);
            return result;
        }
        public async Task RemoveItemFromCartAsync(Guid id, int itemId)
        {
            var cart = await GetCartAsync(id);
            if (cart == null || !cart.Items.Exists(i => i.Id == itemId))
                return;

            cart.Items.RemoveAll(i => i.Id == itemId);
            _db.Update(cart);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                entry.OriginalValues.SetValues(entry.CurrentValues);
            }
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
            _db.Update(item);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                entry.OriginalValues.SetValues(entry.CurrentValues);
            }
        }
    }
}
