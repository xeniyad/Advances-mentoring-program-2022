var builder = DistributedApplication.CreateBuilder(args);

var catalogDb = builder.AddConnectionString("DefaultConnection");
var cartingDb = builder.AddConnectionString("CartingDb");
var orderDb = builder.AddConnectionString("OrderDb");

var catalogService = builder.AddProject<Projects.CatalogService_Web>("catalog-service")
    .WithReference(catalogDb);

var cartService = builder.AddProject<Projects.Carting_API>("cart-service")
    .WithReference(cartingDb);

var orderService = builder.AddProject<Projects.OrderService_Web>("order-service")
    .WithReference(orderDb)
    .WithReference(cartService);

var gateway = builder.AddProject<Projects.ApiGateway_Web>("api-gateway")
    .WithReference(catalogService)
    .WithReference(cartService)
    .WithReference(orderService);

builder.AddNpmApp("frontend", "../frontend", "start")
    .WithReference(gateway)
    .WithEnvironment("REACT_APP_API_URL", gateway.GetEndpoint("http"))
    .WithHttpEndpoint(env: "PORT")
    .ExcludeFromManifest();

builder.Build().Run();
