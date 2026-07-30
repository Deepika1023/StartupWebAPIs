using Microsoft.EntityFrameworkCore;
using StartupWebAPIs.Data;
using StartupWebAPIs.Interfaces;
using StartupWebAPIs.Models;

namespace StartupWebAPIs.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _context.Customers
                .Include(c => c.SubscriptionPlan)
                .ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.SubscriptionPlan)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _context.Customers
                .Include(c => c.SubscriptionPlan)
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Customer> CreateAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return await _context.Customers
                .Include(c => c.SubscriptionPlan)
                .FirstAsync(c => c.Id == customer.Id);
        }

        public async Task<Customer?> UpdateAsync(Customer customer)
        {
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == customer.Id);

            if (existingCustomer == null)
                return null;

            existingCustomer.CompanyName = customer.CompanyName;
            existingCustomer.ContactPerson = customer.ContactPerson;
            existingCustomer.Email = customer.Email;
            existingCustomer.SubscriptionPlanId = customer.SubscriptionPlanId;
            existingCustomer.IsActive = customer.IsActive;

            await _context.SaveChangesAsync();

            return await _context.Customers
                .Include(c => c.SubscriptionPlan)
                .FirstAsync(c => c.Id == customer.Id);
            //return existingCustomer;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return false;

            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}