using Products.Models;
using Products.Services;

namespace Products.Tests.Unit
{
    public class ProductServiceTests
    {
        [Fact]
        public async Task CreateAsync_ShouldAddProduct()
        {
            var db = new TestProductsDbContext();
            var service = new ProductService(db);
            var product = new Product(0, "Hat", "Red", 20m, DateTime.UtcNow);

            var result = await service.CreateAsync(product);

            Assert.Single(await service.GetAllAsync());
            Assert.Equal("Hat", result.Name);
        }

        [Fact]
        public async Task GetByColourAsync_ShouldReturnFiltered()
        {
            var db = new TestProductsDbContext();
            var service = new ProductService(db);
            await service.CreateAsync(new Product(0, "Hat", "Red", 20m, DateTime.UtcNow));
            await service.CreateAsync(new Product(0, "Shirt", "Blue", 30m, DateTime.UtcNow));

            var redItems = await service.GetByColourAsync("Red");

            Assert.Single(redItems);
            Assert.Equal("Hat", redItems.First().Name);
        }
    }

}
