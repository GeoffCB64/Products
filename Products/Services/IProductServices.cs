using Products.Models;

namespace Products.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetByColourAsync(string colour);
        Task<Product> CreateAsync(Product product);
    }
}
