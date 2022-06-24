using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carting.DL
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public Image Image { get; set; }
        [NotMapped]
        public Money Price { get; set; }
        public uint Quantity { get; set; }
        public Cart Cart { get; set; }
    }

    public class ItemsConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            // This Converter will perform the conversion to and from Json to the desired type
            builder.Property(e => e.Image).HasJsonConversion<Image>();
            builder.Property(e => e.Price).HasJsonConversion<Money>();
        }
    }
}
