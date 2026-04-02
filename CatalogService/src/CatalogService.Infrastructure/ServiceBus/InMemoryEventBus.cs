using CatalogService.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace CatalogService.Infrastructure.ServiceBus;

public class InMemoryEventBus : IEventBus
{
    private readonly ILogger<InMemoryEventBus> _logger;

    public InMemoryEventBus(ILogger<InMemoryEventBus> logger)
    {
        _logger = logger;
    }

    public void Publish(IntegrationEvent @event)
    {
        _logger.LogInformation("[InMemoryEventBus] Published {EventType}: {EventId}", @event.GetType().Name, @event.Id);
    }

    public void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
    {
        _logger.LogInformation("[InMemoryEventBus] Subscribed {EventType} -> {HandlerType}", typeof(T).Name, typeof(TH).Name);
    }

    public void SubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler { }

    public void Unsubscribe<T, TH>() where TH : IIntegrationEventHandler<T> where T : IntegrationEvent { }

    public void UnsubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler { }
}
