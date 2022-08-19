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

        public bool EnsureDbCreated()
        {
            using (var db = new CartingContext())
            {
                return db.Database.EnsureCreated();
            }
        }

        public bool IsHasData()
        {
            using (var db = new CartingContext())
            {
                return db.Carts.Any();
            }
        }

        public async Task<Item> AddItemToCartAsync(Guid id, Item item)
        {
            using (var db = new CartingContext())
            {
                var cart = await db.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id);
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
                        db.Items.Add(item);
                    }
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        var entry = ex.Entries.Single();
                        entry.OriginalValues.SetValues(entry.CurrentValues);
                    }
                }
                return item;
            }
        }
        public async Task<Cart> CreateCartAsync(Guid id)
        {
            using (var db = new CartingContext())
            {
                var cart = await GetCartAsync(id);
                if (cart != null)
                    return cart;

                var newCart = new Cart() { Id = id, Items = new List<Item>() };
                db.Carts.Add(newCart);

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.CurrentValues);
                }
                return newCart;
            }
        }
        public async Task<Cart> GetCartAsync(Guid id)
        {
            using (var db = new CartingContext())
            {
                var result = await db.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.Id == id);
                return result;
            }
        }
        public async Task<bool> RemoveItemFromCartAsync(Guid id, int itemId)
        {
            using (var db = new CartingContext())
            {
                var cart = await GetCartAsync(id);
                if (cart == null)
                    return false;
                var item = cart.Items.First(i => i.Id == itemId);

                if (item != null)
                    db.Items.Remove(item);

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.CurrentValues);
                    return false;
                }
                return true;
            }
        }
        public async Task UpdateItemQuantityAsync(Guid id, int itemId, uint quantity)
        {
            using (var db = new CartingContext())
            {
                var cart = await GetCartAsync(id);
                if (cart == null)
                    return;

                var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
                if (item == null)
                    return;
                item.Quantity += quantity;
                db.Update(item);

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.CurrentValues);
                }
            }
        }

        public async Task<IList<Cart>> GetAllCarts()
        {
            using (var db = new CartingContext())
            {
                var result = await db.Carts.Include(c => c.Items).ToListAsync();
                return result;
            }
        }

        public async Task UpdateItemAsync(Guid id, int itemId, Money price, string name)
        {
            using (var db = new CartingContext())
            {
                var cart = await GetCartAsync(id);
                if (cart == null)
                    return;

                var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
                if (item == null)
                    return;
                item.Price =price;
                item.Name = name;
                db.Update(item);

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.CurrentValues);
                }
            }
        }
    }
}
