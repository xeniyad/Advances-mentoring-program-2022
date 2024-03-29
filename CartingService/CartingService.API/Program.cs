using Autofac;
using Autofac.Extensions.DependencyInjection;
using Carting.API;
using Carting.BL;
using Carting.BL.EventBus;
using Carting.BL.IntegrationEvents.EventHandling;
using Carting.BL.IntegrationEvents.Events;
using Carting.DL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Carting.BL.EventBus;



var builder = WebApplication.CreateBuilder(args);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Services.AddDbContext<CartingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CartingDb")));
builder.Services.AddSingleton<IServiceBusPersisterConnection>(sp =>
{
    var serviceBusConnectionString = builder.Configuration["EventBusConnection"];

    return new DefaultServiceBusPersisterConnection(serviceBusConnectionString);
});

Configure.ConfigureServices(builder.Services);
builder.Services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
{
    var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
    var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
    string subscriptionName = builder.Configuration["SubscriptionClientName"];

    return new EventBusServiceBus(serviceBusPersisterConnection,
        eventBusSubcriptionsManager, iLifetimeScope, subscriptionName);
});
// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddControllers();

builder.Services.AddApiVersioning(config =>
{
    config.ReportApiVersions = true;
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.DefaultApiVersion = new ApiVersion(2, 0);
    config.ApiVersionReader = new HeaderApiVersionReader("api-version");
});
builder.Services.AddVersionedApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                });
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
   app.UseItToSeedSqlServer();
//}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwagger();

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
app.UseSwaggerUI(
                options =>
                {
                    options.RoutePrefix = string.Empty;
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());
                    }
                });

app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseAuthorization();

app.MapRazorPages();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
var eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<ItemChangedIntegrationEvent, ItemChangedIntegrationEventHandler>();
app.Run();

namespace Carting.API
{
    public partial class Program { }

}


