using System;
using System.Collections.Generic;

namespace Carting.DL
{
    public class Cart
    {
        public List<Item> Items { get; set; }
        public Guid Id { get; set; }
    }
}
