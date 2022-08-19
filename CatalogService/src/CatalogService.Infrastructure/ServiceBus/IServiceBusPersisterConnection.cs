using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

namespace CatalogService.Infrastructure.ServiceBus;

public interface IServiceBusPersisterConnection : IDisposable
{
    ServiceBusClient TopicClient { get; }
    ServiceBusAdministrationClient AdministrationClient { get; }
}
