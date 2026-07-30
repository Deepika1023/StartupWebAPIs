using StartupWebAPIs.Models;

namespace StartupWebAPIs.Interfaces
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllAsync();

        Task<Customer?> GetByIdAsync(int id);

        Task<Customer?> GetByEmailAsync(string email);

        Task<Customer> CreateAsync(Customer customer);

        Task<Customer?> UpdateAsync(Customer customer);

        Task<bool> DeleteAsync(int id);

    }
}