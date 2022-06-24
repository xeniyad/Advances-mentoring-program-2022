using CatalogService.Core.ProjectAggregate;
using CatalogService.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Infrastructure.Data.Config;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
  public void Configure(EntityTypeBuilder<Item> builder)
  {
    builder.Property(i => i.Name)
      .HasMaxLength(50)
      .IsRequired();

    builder.Property(i => i.Price)
      .IsRequired()
      .HasConversion(
      i => i.ToDbString(),
      i => new Money(i)
      );

    builder.Property(i => i.Amount)
      .IsRequired();
  }
}
