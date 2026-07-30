using StartupWebAPIs.DTOs.Customers;
using StartupWebAPIs.Models;

namespace StartupWebAPIs.Interfaces
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetAllAsync();

        Task<Customer?> GetByIdAsync(int id);

        Task<Customer> RegisterAsync(RegisterCustomerDto dto);

        Task<Customer?> UpdateAsync(int id, Customer customer);

        Task<bool> DeleteAsync(int id);

        Task<string?> LoginAsync(LoginCustomerDto dto);
    }
}