using StartupWebAPIs.Interfaces;
using StartupWebAPIs.Models;

namespace StartupWebAPIs.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _repository.GetAllAsync();
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