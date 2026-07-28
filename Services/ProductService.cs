using StartupWebAPIs.Controllers;
using StartupWebAPIs.Interfaces;
using StartupWebAPIs.Models;
using Microsoft.Extensions.Caching.Memory;
using StartupWebAPIs.Helpers;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
namespace StartupWebAPIs.Services

{

    public class ProductService : IProductService
    {
        //private const string ProductCacheKey = "products_cache";
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductService> _logger;
        // private readonly IMemoryCache _cache;
        private readonly IDistributedCache _cache;

        public ProductService(IProductRepository repository, ILogger<ProductService> logger, IDistributedCache cache)
        {
            _repository = repository;
            _logger = logger;
            _cache = cache;
        }

        // public async Task<IEnumerable<Product>> GetAllProductsAsync(string? search, string? sort)
        public async Task<IEnumerable<Product>> GetAllProductsAsync(string? search, string? sort, decimal? minPrice, decimal? maxPrice)
        {
            var cachedProducts = await _cache.GetStringAsync(CacheKeys.Products);
            List<Product>? products;

            if (string.IsNullOrEmpty(cachedProducts))
            {
                _logger.LogInformation("Cache MISS");

                products = (await _repository.GetAllAsync()).ToList();

                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };

                var serializedProducts = JsonSerializer.Serialize(products);
                await _cache.SetStringAsync(CacheKeys.Products, serializedProducts, cacheOptions);
                var verify = await _cache.GetStringAsync(CacheKeys.Products);

                _logger.LogInformation("Redis Verify = {Verify}", verify != null ? "SUCCESS" : "FAILED");
              //  _logger.LogInformation("Products stored in Redis successfully.");
            }
            else
            {
                _logger.LogInformation("Cache HIT");
                products = JsonSerializer.Deserialize<List<Product>>(cachedProducts);
            }

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                products = products.Where(p =>
                    p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Minimum Price
            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value).ToList();
            }

            // Maximum Price
            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value).ToList();
            }

            // Sorting
            products = sort?.ToLower() switch
            {
                "name" => products.OrderBy(p => p.Name).ToList(),
                "name_desc" => products.OrderByDescending(p => p.Name).ToList(),
                "price" => products.OrderBy(p => p.Price).ToList(),
                "price_desc" => products.OrderByDescending(p => p.Price).ToList(),
                _ => products.OrderBy(p => p.Id).ToList()
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
            await _cache.RemoveAsync(CacheKeys.Products);

            _logger.LogInformation("Product cache cleared after Created a product.");
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
            await _cache.RemoveAsync(CacheKeys.Products);

            _logger.LogInformation("Product cache cleared after updated a product.");
            return existing;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
                return false;

            _repository.Delete(product);

            await _repository.SaveAsync();

            await _cache.RemoveAsync(CacheKeys.Products);

            _logger.LogInformation("Product cache cleared after deleting a product.");

            return true;
        }
    }
}