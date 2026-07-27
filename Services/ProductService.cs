using StartupWebAPIs.Controllers;
using StartupWebAPIs.Interfaces;
using StartupWebAPIs.Models;

namespace StartupWebAPIs.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository repository, ILogger<ProductService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

       // public async Task<IEnumerable<Product>> GetAllProductsAsync(string? search, string? sort)
        public async Task<IEnumerable<Product>> GetAllProductsAsync(string? search,string? sort,decimal? minPrice,decimal? maxPrice)
        {
            var products = await _repository.GetAllAsync();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                products = products.Where(p =>
                    p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));

            }
            // Minimum Price
            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }

            // Maximum Price
            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }


            // Sorting
            products = sort?.ToLower() switch
            {
                "name" => products.OrderBy(p => p.Name),

                "name_desc" => products.OrderByDescending(p => p.Name),

                "price" => products.OrderBy(p => p.Price),

                "price_desc" => products.OrderByDescending(p => p.Price),

                _ => products.OrderBy(p => p.Id)
            };
            try
            {
                // database operation
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while creating product");

                throw;
            }
            return products;

        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            await _repository.AddAsync(product);
            await _repository.SaveAsync();

            return product;
        }

        public async Task<Product?> UpdateProductAsync(int id, Product product)
        {
            var existing = await _repository.GetByIdAsync(id);

            if (existing == null)
                return null;

            existing.Name = product.Name;
            existing.Price = product.Price;

            _repository.Update(existing);

            await _repository.SaveAsync();

            return existing;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
                return false;

            _repository.Delete(product);

            await _repository.SaveAsync();

            return true;
        }
    }
}