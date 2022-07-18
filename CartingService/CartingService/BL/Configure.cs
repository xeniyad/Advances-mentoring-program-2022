using Carting.DL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}
