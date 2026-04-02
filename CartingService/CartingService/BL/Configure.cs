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
            services.AddScoped<CartingService>();

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            services.AddTransient<ItemChangedIntegrationEventHandler>();
        }
    }
}
