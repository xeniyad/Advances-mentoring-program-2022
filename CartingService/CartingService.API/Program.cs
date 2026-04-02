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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Services.AddDbContext<CartingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CartingDb")));

var serviceBusConnectionString = builder.Configuration["EventBusConnection"];
var serviceBusEnabled = !string.IsNullOrWhiteSpace(serviceBusConnectionString);

if (serviceBusEnabled)
{
    builder.Services.AddSingleton<IServiceBusPersisterConnection>(sp =>
        new DefaultServiceBusPersisterConnection(serviceBusConnectionString));
}

Configure.ConfigureServices(builder.Services);

if (serviceBusEnabled)
{
    builder.Services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
    {
        var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
        var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
        var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
        string? subscriptionName = builder.Configuration["SubscriptionClientName"];

        return new EventBusServiceBus(serviceBusPersisterConnection,
            eventBusSubcriptionsManager, iLifetimeScope, subscriptionName);
    });
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

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

app.UseItToSeedSqlServer();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}
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

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapDefaultEndpoints();

if (serviceBusEnabled)
{
    var eventBus = app.Services.GetRequiredService<IEventBus>();
    eventBus.Subscribe<ItemChangedIntegrationEvent, ItemChangedIntegrationEventHandler>();
}

app.Run();

namespace Carting.API
{
    public partial class Program { }
}
