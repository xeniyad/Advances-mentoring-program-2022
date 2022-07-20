using System;
using System.Threading.Tasks;

namespace Carting.DL
{
    public interface ICartingRepository
    {
        Task<Cart> CreateCartAsync(Guid id);
        Task<Cart> GetCartAsync(Guid id);
        Task<Item> AddItemToCartAsync(Guid id, Item item);
        Task<bool> RemoveItemFromCartAsync(Guid id, int itemId);
        Task UpdateItemQuantityAsync(Guid id, int itemId, uint quantity);
        bool EnsureDbCreated();
        bool IsHasData();
    }
}
