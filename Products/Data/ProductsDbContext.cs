using Microsoft.EntityFrameworkCore;
using Products.Models;


namespace Products.Data
{
    public class ProductsDbContext : DbContext
    {
        public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();
    }
}
