using Carting.DL;

namespace Carting.BL.DTO
{
    public class ItemDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Image Image { get; set; }
        public Money Price { get; set; }
        public uint Quantity { get; set; }
    }

}
