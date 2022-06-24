using CatalogService.Core.ProjectAggregate;
using CatalogService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using CatalogService.Core.ValueObjects;

namespace CatalogService.UnitTests.Core.CategoryAggregate;

public static class SeedData
{
  public static readonly Category TestCategory1 = new Category("Test Project");
  public static readonly Item Item1 = new Item
  {
    Name = "Bread",
    Description = "Delicious white classic bread",
    Image = "https://images.pexels.com/photos/90946/pexels-photo-90946.jpeg?auto=compress&cs=tinysrgb&w=600",
    Price = new Money(100.0, Currency.USD),
    Amount = 5
  };
  public static readonly Item Item2 = new Item
  {
    Name = "Butter",
    Description = "The best butter ever",
    Image = "https://images.pexels.com/photos/90946/pexels-photo-90946.jpeg?auto=compress&cs=tinysrgb&w=600",
    Price = new Money(100.0, Currency.USD),
    Amount = 5
  };
  public static readonly Item Item3 = new Item
  {
    Name = "Milk",
    Description = "3% farm milk",
    Image = "https://images.pexels.com/photos/90946/pexels-photo-90946.jpeg?auto=compress&cs=tinysrgb&w=600",
    Price = new Money(100.0, Currency.USD),
    Amount = 5
  };

  public static void PopulateTestData(AppDbContext dbContext)
  {
    foreach (var item in dbContext.Categories)
    {
      dbContext.Remove(item);
    }
    foreach (var item in dbContext.Categories)
    {
      dbContext.Remove(item);
    }
    dbContext.SaveChanges();

    TestCategory1.AddItem(Item1);
    TestCategory1.AddItem(Item2);
    TestCategory1.AddItem(Item3);
    dbContext.Categories.Add(TestCategory1);

    dbContext.SaveChanges();
  }
}
