using CatalogService.Core.Interfaces;

namespace CatalogService.Web.Integration;

public interface ICatalogIntegrationEventService
{
    Task PublishThroughEventBusAsync(IntegrationEvent evt);
}
