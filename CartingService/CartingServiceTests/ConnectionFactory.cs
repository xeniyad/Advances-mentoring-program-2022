using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carting.Tests
{
    public class CartingContextConnectionFactory : IDisposable
    {
        private DbConnection _connection;

        private DbContextOptions<CartingContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<CartingContext>()
                .UseSqlite(_connection).Options;
        }

        public CartingContext CreateContext()
        {
            if (_connection == null)
            {
                _connection = new SqliteConnection("DataSource=:memory:");
                _connection.Open();

                var options = CreateOptions();
                using (var context = new CartingContext(options))
                {
                    context.Database.EnsureCreated();
                }
            }

            return new CartingContext(CreateOptions());
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
