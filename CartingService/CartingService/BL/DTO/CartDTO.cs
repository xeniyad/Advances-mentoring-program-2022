using System;
using System.Collections.Generic;

namespace Carting.BL.DTO
{
    public class CartDTO
    {
        public List<ItemDTO> Items { get; set; }
        public string Id { get; set; }
    }
}
