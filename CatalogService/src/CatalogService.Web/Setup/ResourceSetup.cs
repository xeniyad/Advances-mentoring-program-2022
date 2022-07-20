using System;
using CatalogService.Web.Models.Categories;
using CatalogService.Web.Models.Items;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogService.Web.Setup
{
    public static class ResourceSetup
    {
        public static IServiceCollection ConfigureResources(this IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            return services
                .AddScoped<CategoryResourceFactory>()
                .AddScoped<ItemResourceFactory>();
        }
    }
}
