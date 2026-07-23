using StartupWebAPIs.Interfaces;

public interface IUnitOfWork
{
    IProductRepository Products { get; }

  //  IUserRepository Users { get; }

    Task<int> SaveChangesAsync();
}