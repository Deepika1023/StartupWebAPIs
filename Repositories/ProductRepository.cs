using StartupWebAPIs.Data;
using StartupWebAPIs.Interfaces;
using StartupWebAPIs.Models;

namespace StartupWebAPIs.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context)
            : base(context)
        {
        }
    }
}