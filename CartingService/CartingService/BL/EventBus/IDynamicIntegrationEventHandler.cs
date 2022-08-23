using System.Threading.Tasks;

namespace Carting.BL.EventBus;

public interface IDynamicIntegrationEventHandler
{
    Task Handle(dynamic eventData);
}
