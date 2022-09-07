using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.Reflection;

namespace Carting.DL
{
    public class CartingContext : DbContext, IDisposable
    {
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Item> Items { get; set; }
        public string DbPath { get; }

        public CartingContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
