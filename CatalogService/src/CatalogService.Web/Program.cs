﻿using System.Configuration;
using Ardalis.ListStartupServices;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using CatalogService.Core;
using CatalogService.Core.Interfaces;
using CatalogService.Infrastructure;
using CatalogService.Infrastructure.Data;
using CatalogService.Infrastructure.ServiceBus;
using CatalogService.Web;
using CatalogService.Web.Integration;
using CatalogService.Web.Setup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Owin.Security.OpenIdConnect;
using NuGet.Configuration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
 options.AddDefaultPolicy(
                   policy =>
                    {
                      policy.WithOrigins("https://login.microsoftonline.com/", "https://localhost:7105");
                    });
});

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

builder.Services.AddSingleton<IServiceBusPersisterConnection>(sp =>
{
  var serviceBusConnectionString = builder.Configuration["EventBusConnection"];

  return new DefaultServiceBusPersisterConnection(serviceBusConnectionString);
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
  options.CheckConsentNeeded = context => true;
  options.MinimumSameSitePolicy = SameSiteMode.None;
});

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); //builder.Configuration.GetConnectionString("SqliteConnection"); 

builder.Services.AddDbContext(connectionString);
builder.Services.AddResponseCaching();
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdB2C"));

builder.Services.AddControllersWithViews(options =>
{
  var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
  options.Filters.Add(new AuthorizeFilter(policy)); 
  options.CacheProfiles.Add("Default10",
      new CacheProfile()
      {
        Duration = 10
      });
}).AddMicrosoftIdentityUI(); 
builder.Services.AddRazorPages();
builder.Services.ConfigureApi();
builder.Services.AddTransient<ICatalogIntegrationEventService, CatalogIntegrationEventService>();
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

builder.Services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

// add list services for diagnostic purposes - see https://github.com/ardalis/AspNetCoreStartupServices
builder.Services.Configure<ServiceConfig>(config =>
{
  config.Services = new List<ServiceDescriptor>(builder.Services);

  // optional - default path to view services is /listallservices - recommended to choose your own path
  config.Path = "/listservices";
});


builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
  containerBuilder.RegisterModule(new DefaultInfrastructureModule(builder.Environment.EnvironmentName == "Development"));
});

//builder.Logging.AddAzureWebAppDiagnostics(); add this if deploying to Azure
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
  c.EnableAnnotations();
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  app.UseShowAllServicesMiddleware();
}
else
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}

app.UseCors(builder => builder
     .AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader());
app.UseHttpsRedirection();
app.UseRouting();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));

app.UseEndpoints(endpoints =>
{
  endpoints.MapDefaultControllerRoute();
  endpoints.MapRazorPages();
});

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
