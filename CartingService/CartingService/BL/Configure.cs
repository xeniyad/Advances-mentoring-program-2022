using Autofac;
using Carting.BL.EventBus;
using Carting.BL.IntegrationEvents.EventHandling;
using Carting.DL;
using Microsoft.Extensions.DependencyInjection;

namespace Carting.BL
{
    public static class Configure
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ICartingRepository, CartingRepository>();
            services.AddScoped<CartingContext, CartingContext>();
            services.AddDbContext<CartingContext>();
            services.AddScoped<DbInitializer>();

            

            RegisterEventBus(services);
        }

        private static void RegisterEventBus(IServiceCollection services)
        {
                services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
                {
                    var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                    string subscriptionName = "Cart";

                    return new EventBusServiceBus(serviceBusPersisterConnection,
                        eventBusSubcriptionsManager, iLifetimeScope, subscriptionName);
                });
            

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            services.AddTransient<ItemChangedIntegrationEventHandler>();
        }
    }
}
