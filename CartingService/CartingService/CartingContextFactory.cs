using Carting.DL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace CatalogService.Infrastructure;

public class CartingContextFactory : IDesignTimeDbContextFactory<CartingContext>
{
    public CartingContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CartingContext>();
        optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=carting;Trusted_Connection=True;");

        return new CartingContext(optionsBuilder.Options);
    }
}
