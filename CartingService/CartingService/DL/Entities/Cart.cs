using System;
using System.Collections.Generic;

namespace Carting.DL
{
    public class Cart
    {
        public Cart()
        {
            Items = new List<Item>();
        }
        public List<Item> Items { get; set; }
        public Guid Id { get; set; }
    }
}
