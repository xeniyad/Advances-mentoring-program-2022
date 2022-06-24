using System;
using System.Threading.Tasks;

namespace Carting.DL
{
    public interface ICartingRepository
    {
        Task<Cart> CreateCartAsync(Guid id);
        Task<Cart> GetCartAsync(Guid id);
        Task AddItemToCartAsync(Guid id, Item item);
        Task RemoveItemFromCartAsync(Guid id, int itemId);
        Task UpdateItemQuantityAsync(Guid id, int itemId, uint quantity);
    }
}
