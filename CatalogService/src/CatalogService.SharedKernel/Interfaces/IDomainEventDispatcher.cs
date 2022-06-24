
namespace CatalogService.SharedKernel.Interfaces;

public interface IDomainEventDispatcher
{
  Task DispatchAndClearEvents(IEnumerable<EntityBase> entitiesWithEvents);
}
