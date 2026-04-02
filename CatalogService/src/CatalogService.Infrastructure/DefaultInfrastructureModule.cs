using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CatalogService.Core.Interfaces;
using CatalogService.Core.ProjectAggregate;
using CatalogService.Infrastructure.Data;
using CatalogService.Infrastructure.Services;
using CatalogService.SharedKernel;
using CatalogService.SharedKernel.Interfaces;
using MediatR;
using Module = Autofac.Module;

namespace CatalogService.Infrastructure;

public class DefaultInfrastructureModule : Module
{
  private readonly bool _isDevelopment;
  private readonly List<Assembly> _assemblies = new();

  public DefaultInfrastructureModule(bool isDevelopment, Assembly? callingAssembly = null)
  {
    _isDevelopment = isDevelopment;
    var coreAssembly = Assembly.GetAssembly(typeof(Category));
    var infrastructureAssembly = Assembly.GetAssembly(typeof(StartupSetup));
    if (coreAssembly != null) _assemblies.Add(coreAssembly);
    if (infrastructureAssembly != null) _assemblies.Add(infrastructureAssembly);
    if (callingAssembly != null) _assemblies.Add(callingAssembly);
  }

  protected override void Load(ContainerBuilder builder)
  {
    if (_isDevelopment)
      RegisterDevelopmentOnlyDependencies(builder);
    else
      RegisterProductionOnlyDependencies(builder);

    RegisterCommonDependencies(builder);
  }

  private void RegisterCommonDependencies(ContainerBuilder builder)
  {
    builder.Register<IMediator>(ctx =>
    {
      var scope = ctx.Resolve<ILifetimeScope>();
      return new Mediator(new AutofacServiceProvider(scope));
    }).InstancePerLifetimeScope();

    builder.RegisterType<DomainEventDispatcher>()
      .As<IDomainEventDispatcher>()
      .InstancePerLifetimeScope();

    builder.RegisterType<CategoryService>()
      .As<ICategoryService>()
      .InstancePerLifetimeScope();

    builder.RegisterType<ItemService>()
      .As<IItemService>()
      .InstancePerLifetimeScope();

    var mediatrOpenTypes = new[]
    {
      typeof(IRequestHandler<,>),
      typeof(INotificationHandler<>),
    };

    foreach (var mediatrOpenType in mediatrOpenTypes)
    {
      builder.RegisterAssemblyTypes(_assemblies.ToArray())
        .AsClosedTypesOf(mediatrOpenType)
        .AsImplementedInterfaces();
    }
  }

  private void RegisterDevelopmentOnlyDependencies(ContainerBuilder builder) { }
  private void RegisterProductionOnlyDependencies(ContainerBuilder builder) { }
}
