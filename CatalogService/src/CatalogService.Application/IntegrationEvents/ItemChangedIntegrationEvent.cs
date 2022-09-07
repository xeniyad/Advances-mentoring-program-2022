using CatalogService.Core.Interfaces;
using CatalogService.Core.ValueObjects;

namespace CatalogService.Application.IntegrationEvents;

// Integration Events notes: 
// An Event is “something that has happened in the past”, therefore its name has to be past tense
// An Integration Event is an event that can cause side effects to other microservices, Bounded-Contexts or external systems.
public record ItemChangedIntegrationEvent : IntegrationEvent
{
    public int ProductId { get; private init; }

    public Money NewPrice { get; private init; }

    public string NewName { get; private init; }

  public ItemChangedIntegrationEvent(int productId, Money newPrice, string newName)
    {
        ProductId = productId;
        NewPrice = newPrice;
        NewName = newName;
  }
}
