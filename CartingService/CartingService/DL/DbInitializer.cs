using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carting.DL
{
    public class DbInitializer
    {
        internal async static void Initialize(CartingContext context)
        {
            context.Database.EnsureCreated();
            if (context.Carts.Any()) return;

            var cart1Id = new Guid("062383c6-60b0-474f-804a-8ee26d8f7868");
            var cart2Id = new Guid("b240b546-10a9-4f59-9e3c-6cae483e208e");

            var cart1 = new Cart();
            cart1.Id = cart1Id;
            context.Carts.Add(cart1);
            var item1 = new Item
            {
                Name = "Bread",
                Price = new Money(2.2, Currency.USD),
                Quantity = 2,
                Image = null
            };
            var item2 = new Item
            {
                Name = "Butter",
                Price = new Money(20.51, Currency.USD),
                Quantity = 4,
                Image = null
            };
            cart1.Items.Add(item1);
            cart1.Items.Add(item2);

            var cart2 = new Cart();
            cart2.Id = cart2Id;
            context.Carts.Add(cart2);
            context.SaveChanges();
        }
    }
}
