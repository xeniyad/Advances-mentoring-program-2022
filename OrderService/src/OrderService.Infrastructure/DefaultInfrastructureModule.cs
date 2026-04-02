using Autofac;
using OrderService.Core.Interfaces;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Services;

namespace OrderService.Infrastructure;

public class DefaultInfrastructureModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<OrderRepository>()
            .As<IOrderRepository>()
            .InstancePerLifetimeScope();

        builder.RegisterType<OrderService.Infrastructure.Services.OrderService>()
            .As<IOrderService>()
            .InstancePerLifetimeScope();
    }
}
