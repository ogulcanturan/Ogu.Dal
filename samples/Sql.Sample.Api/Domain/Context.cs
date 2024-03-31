using Microsoft.EntityFrameworkCore;
using Sql.Sample.Api.Domain.Entities;

namespace Sql.Sample.Api.Domain
{
    public sealed class Context : DbContext
    {
        public Context(DbContextOptions<Context> opts) : base(opts) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}