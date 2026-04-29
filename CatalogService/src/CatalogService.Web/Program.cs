using System.Configuration;
using Ardalis.ListStartupServices;
using CatalogService.Core;
using CatalogService.Core.Interfaces;
using CatalogService.Infrastructure;
using CatalogService.Infrastructure.Data;
using CatalogService.Infrastructure.ServiceBus;
using CatalogService.Infrastructure.Services;
using CatalogService.SharedKernel;
using CatalogService.SharedKernel.Interfaces;
using CatalogService.Web;
using CatalogService.Web.Integration;
using CatalogService.Web.Setup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Allow HTTP/1.1 connections (needed when gateway proxies over plain HTTP)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureEndpointDefaults(o =>
        o.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2);
});

builder.AddServiceDefaults();

builder.Services.AddCors(options =>
{
 options.AddDefaultPolicy(
                   policy =>
                    {
                      policy.WithOrigins("https://login.microsoftonline.com/", "https://localhost:7105", "https://localhost:5000", "http://localhost:51923");
                    });
});

builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

var serviceBusConnectionString = builder.Configuration["EventBusConnection"];
var serviceBusEnabled = !string.IsNullOrWhiteSpace(serviceBusConnectionString);

if (serviceBusEnabled)
{
  builder.Services.AddSingleton<IServiceBusPersisterConnection>(sp =>
      new DefaultServiceBusPersisterConnection(serviceBusConnectionString));
}

builder.Services.Configure<CookiePolicyOptions>(options =>
{
  options.CheckConsentNeeded = context => true;
  options.MinimumSameSitePolicy = SameSiteMode.None;
});

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); //builder.Configuration.GetConnectionString("SqliteConnection"); 

builder.Services.AddDbContext(connectionString);
builder.Services.AddResponseCaching();
var azureAd = builder.Configuration.GetSection("AzureAdB2C");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
          if (builder.Environment.IsDevelopment())
          {
            // In dev, bypass all JWT validation and parse mock tokens directly.
            // SignatureValidator / TokenValidationParameters are unreliable with
            // alg:none tokens on .NET 9's JsonWebTokenHandler pipeline.
            options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
            {
              OnMessageReceived = context =>
              {
                var authHeader = context.Request.Headers.Authorization.ToString();
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                  var raw = authHeader["Bearer ".Length..];
                  var jwt = new Microsoft.IdentityModel.JsonWebTokens.JsonWebToken(raw);
                  var claims = jwt.Claims.ToList();
                  var identity = new System.Security.Claims.ClaimsIdentity(claims, "Bearer", "name", "roles");
                  context.Principal = new System.Security.Claims.ClaimsPrincipal(identity);
                  context.Success();
                }
                return Task.CompletedTask;
              }
            };
          }
          else
          {
            options.Authority = $"{azureAd["Instance"]}{azureAd["Domain"]}/{azureAd["SignUpSignInPolicyId"]}/v2.0";
            options.Audience = azureAd["ClientId"];
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
              RoleClaimType = "roles",
            };
          }
        });

builder.Services.AddControllersWithViews(options =>
{
  options.CacheProfiles.Add("Default10",
      new CacheProfile()
      {
        Duration = 10
      });
});
builder.Services.AddRazorPages();
builder.Services.ConfigureApi();
builder.Services.AddTransient<ICatalogIntegrationEventService, CatalogIntegrationEventService>();
builder.Services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

if (serviceBusEnabled)
{
  builder.Services.AddSingleton<EventBusServiceBus>(sp =>
  {
    var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
    string subscriptionName = builder.Configuration["SubscriptionClientName"] ?? "catalogservice";

    return new EventBusServiceBus(serviceBusPersisterConnection,
        eventBusSubcriptionsManager, scopeFactory, subscriptionName);
  });
  builder.Services.AddSingleton<IEventBus>(sp => sp.GetRequiredService<EventBusServiceBus>());
  builder.Services.AddHostedService(sp => sp.GetRequiredService<EventBusServiceBus>());
}
else
{
  builder.Services.AddSingleton<IEventBus, InMemoryEventBus>();
}

// add list services for diagnostic purposes - see https://github.com/ardalis/AspNetCoreStartupServices
builder.Services.Configure<ServiceConfig>(config =>
{
  config.Services = new List<ServiceDescriptor>(builder.Services);

  // optional - default path to view services is /listallservices - recommended to choose your own path
  config.Path = "/listservices";
});


// Services previously registered via Autofac DefaultInfrastructureModule
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CatalogService.Core.ProjectAggregate.Category>());
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IItemService, ItemService>();

//builder.Logging.AddAzureWebAppDiagnostics(); add this if deploying to Azure
builder.Services.ConfigureSwagger();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  app.UseShowAllServicesMiddleware();
}
else
{
  app.UseExceptionHandler("/Home/Error");
 // app.UseHsts();
}

//if (!app.Environment.IsDevelopment())
//{
//  app.UseHttpsRedirection();
//}
app.UseRouting();
app.UseCors(builder => builder
     .AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader());
app.UseStaticFiles();
app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();

app.UseCustomSwagger();

app.MapControllers();
app.MapDefaultControllerRoute();
app.MapRazorPages();
app.MapDefaultEndpoints();

// Seed Database
using (var scope = app.Services.CreateScope())
{
  var services = scope.ServiceProvider;

  try
  {
    var context = services.GetRequiredService<AppDbContext>();
    //                    context.Database.Migrate();
    context.Database.EnsureCreated();
    SeedData.Initialize(services);
  }
  catch (Exception ex)
  {
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
  }
}

app.Run();

namespace CatalogService.Web
{
  public partial class Program { }
}
