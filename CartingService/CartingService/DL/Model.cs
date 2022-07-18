using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;

namespace Carting.DL
{
    public class CartingContext : DbContext, IDisposable
    {
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Item> Items { get; set; }
        public string DbPath { get; }
        public CartingContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "carting.db");
        }

        public CartingContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
    }
}
