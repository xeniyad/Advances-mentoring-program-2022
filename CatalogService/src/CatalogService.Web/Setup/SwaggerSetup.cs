﻿using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace CatalogService.Web.Setup
{
    internal static class SwaggerSetup
    {
        internal static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            return services.AddSwaggerGen(setup =>
            {
                setup.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "Categories Api",
                        Version = "v1",
                        Description = "An API that facilitates catalog items",
                        TermsOfService = new Uri("http://example.com/terms"),
                        Contact = new OpenApiContact
                        {
                            Name = "Developer",
                            Email = "developer@example.com"
                        },
                        License = new OpenApiLicense
                        {
                            Name = "Apache 2.0",
                            Url = new Uri("http://www.apache.org/licenses/LICENSE-2.0.html")
                        }
                    });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                setup.EnableAnnotations();
                setup.IncludeXmlComments(xmlPath);
            });
        }

        internal static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(setup =>
            {
                setup.SwaggerEndpoint("/swagger/v1/swagger.json", "Categories Api V1");
                setup.RoutePrefix = string.Empty;
                setup.DefaultModelExpandDepth(2);
                setup.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Example);
                setup.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                setup.EnableDeepLinking();
            });
            return app;
        }
    }
}
