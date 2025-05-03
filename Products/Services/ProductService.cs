using Microsoft.EntityFrameworkCore;
using Products.Data;
using Products.Models;

namespace Products.Services
{
    public class ProductService : IProductService
    {
        private readonly ProductsDbContext _context;

        public ProductService(ProductsDbContext context) => _context = context;

        public async Task<IEnumerable<Product>> GetAllAsync() =>
            await _context.Products.ToListAsync();

        public async Task<IEnumerable<Product>> GetByColourAsync(string colour) =>
            await _context.Products.Where(p => p.Colour.Equals(colour, StringComparison.OrdinalIgnoreCase)).ToListAsync();

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }
    }
}
