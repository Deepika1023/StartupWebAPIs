using StartupWebAPIs.Data;
using StartupWebAPIs.Interfaces;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IProductRepository Products { get; }

    //public IUserRepository Users { get; }

    //public UnitOfWork(AppDbContext context,
    //                  IProductRepository productRepository,
    //                  IUserRepository userRepository)
    //{
    //    _context = context;

    //    Products = productRepository;

    //    Users = userRepository;
    //}

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}