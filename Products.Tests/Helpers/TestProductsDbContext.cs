using Microsoft.EntityFrameworkCore;
using Products.Data;

namespace Products.Tests.Unit;

public class TestProductsDbContext : ProductsDbContext
{
    public TestProductsDbContext() : base(
        new DbContextOptionsBuilder<ProductsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options)
    {
        Database.EnsureCreated();
    }
}

