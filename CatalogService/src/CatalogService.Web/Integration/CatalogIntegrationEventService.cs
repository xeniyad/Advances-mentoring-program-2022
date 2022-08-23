using CatalogService.Core.Interfaces;

namespace CatalogService.Web.Integration;

public class CatalogIntegrationEventService : ICatalogIntegrationEventService
{
    private readonly IEventBus _eventBus;
  private const string _appName = "Catalog service";
    private readonly ILogger<CatalogIntegrationEventService> _logger;

  public object ResilientTransaction { get; private set; }

  public CatalogIntegrationEventService(
        ILogger<CatalogIntegrationEventService> logger,
        IEventBus eventBus)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
    {
        try
        {
            _logger.LogInformation("----- Publishing integration event: {IntegrationEventId_published} from {AppName} - ({@IntegrationEvent})", evt.Id, _appName, evt);

            _eventBus.Publish(evt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", evt.Id, _appName, evt);
        }
    }

}
