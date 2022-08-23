using Carting.BL.IntegrationEvents.Events;
using Carting.DL;
using System.Linq;
using System.Threading.Tasks;

namespace Carting.BL.IntegrationEvents.EventHandling;

public class ItemChangedIntegrationEventHandler : IIntegrationEventHandler<ItemChangedIntegrationEvent>
{
    private readonly CartingService _service;

    public ItemChangedIntegrationEventHandler(
        ICartingRepository repository)
    {
        _service = new CartingService(repository);
    }

    public async Task Handle(ItemChangedIntegrationEvent @event)
    {       
            var carts = await _service.GetAllCartsAsync();

            foreach (var cart in carts)
            {
                await UpdateItemInCartItems(@event.ProductId, @event.NewPrice, @event.NewName, cart);
            }
    }

    private async Task UpdateItemInCartItems(int productId, Money newPrice, string newName, Cart cart)
    {
        var itemsToUpdate = cart?.Items?.Where(x => x.Id == productId).ToList();

        if (itemsToUpdate != null)
        {
            foreach (var item in itemsToUpdate)
            {
                item.Price = newPrice;
                item.Name = newName;
            }
            await _service.UpdateItemAsync(cart.Id, productId, newPrice, newName);
        }
    }
}
