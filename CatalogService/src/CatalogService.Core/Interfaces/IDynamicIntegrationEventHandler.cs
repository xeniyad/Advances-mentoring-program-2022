namespace CatalogService.Core.Interfaces;

public interface IDynamicIntegrationEventHandler
{
    Task Handle(dynamic eventData);
}
