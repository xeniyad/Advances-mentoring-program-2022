using Carting.DL;
using Microsoft.Extensions.DependencyInjection;
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
        }
    }
}
