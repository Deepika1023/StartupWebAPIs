using StartupWebAPIs.Models;

namespace StartupWebAPIs.Interfaces
{
    public interface IProductService
    {
        //Task<IEnumerable<Product>> GetAllProductsAsync();
       // Task<IEnumerable<Product>> GetAllProductsAsync(string? search, string? sort);
        Task<IEnumerable<Product>> GetAllProductsAsync( string? search,string? sort,decimal? minPrice,decimal? maxPrice);

        Task<Product?> GetProductByIdAsync(int id);

        Task<Product> CreateProductAsync(Product product);

        Task<Product?> UpdateProductAsync(int id, Product product);

        Task<bool> DeleteProductAsync(int id);
    }
}