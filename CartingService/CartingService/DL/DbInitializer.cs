using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carting.DL
{
    public class DbInitializer
    {
        internal static void Initialize(ICartingRepository repository)
        {
            repository.EnsureDbCreated();
            if (repository.IsHasData()) return;

            var cart1Id = new Guid("062383c6-60b0-474f-804a-8ee26d8f7868");
            var cart2Id = new Guid("b240b546-10a9-4f59-9e3c-6cae483e208e");

            repository.CreateCartAsync(cart1Id);
            repository.CreateCartAsync(cart2Id);

            repository.AddItemToCartAsync(cart1Id, new Item
            {
                Name = "Bread",
                Id = 1,
                Price = new Money(2.2, Currency.USD),
                Quantity = 2,
                Image = null
            });
            repository.AddItemToCartAsync(cart1Id, new Item
            {
                Name = "Butter",
                Id = 2,
                Price = new Money(20.51, Currency.USD),
                Quantity = 4,
                Image = null
            });
        }
    }
}
