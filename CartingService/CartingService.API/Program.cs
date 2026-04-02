using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Carting.API;
using Carting.BL;
using Carting.BL.EventBus;
using Carting.BL.IntegrationEvents.EventHandling;
using Carting.BL.IntegrationEvents.Events;
using Carting.DL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

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
    string? subscriptionName = builder.Configuration["SubscriptionClientName"];

    return new EventBusServiceBus(serviceBusPersisterConnection,
        eventBusSubcriptionsManager, iLifetimeScope, subscriptionName);
});

builder.Services.AddControllers();

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(2, 0);
    options.ApiVersionReader = new HeaderApiVersionReader("api-version");
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler("/Error");
app.UseHsts();
app.UseItToSeedSqlServer();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwagger();

var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
app.UseSwaggerUI(options =>
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
app.UseAuthorization();
app.MapControllers();

var eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<ItemChangedIntegrationEvent, ItemChangedIntegrationEventHandler>();
app.Run();

namespace Carting.API
{
    public partial class Program { }
}
