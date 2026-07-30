using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StartupWebAPIs.Data;
using StartupWebAPIs.DTOs.Customers;
using StartupWebAPIs.Interfaces;
using StartupWebAPIs.Models;
using Microsoft.AspNetCore.Identity;
using StartupWebAPIs.DTOs.Customers;
using StartupWebAPIs.Interfaces;
using StartupWebAPIs.Models;

namespace StartupWebAPIs.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly AppDbContext _context;
        private readonly PasswordHasher<Customer> _passwordHasher;
        private readonly IJwtService _jwtService;

        public CustomerService(
            ICustomerRepository repository,
            AppDbContext context, IJwtService jwtService)
        {
            _repository = repository;
            _context = context;
            _passwordHasher = new PasswordHasher<Customer>();
            _jwtService = jwtService;
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Customer> RegisterAsync(RegisterCustomerDto dto)
        {
            // Check duplicate email
            var existing = await _repository.GetByEmailAsync(dto.Email);

            if (existing != null)
                throw new Exception("Email already exists.");

            // Get Free subscription plan
            var freePlan = await _context.SubscriptionPlans
                .FirstAsync(x => x.Name == "Free");

            var customer = new Customer
            {
                CompanyName = dto.CompanyName,
                ContactPerson = dto.ContactPerson,
                Email = dto.Email,
                SubscriptionPlanId = freePlan.Id,
                IsActive = true
            };

            customer.PasswordHash =
                _passwordHasher.HashPassword(customer, dto.Password);

            return await _repository.CreateAsync(customer);
        }

        public async Task<Customer?> UpdateAsync(int id, Customer customer)
        {
            customer.Id = id;
            return await _repository.UpdateAsync(customer);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<string?> LoginAsync(LoginCustomerDto dto)
        {
            var customer = await _repository.GetByEmailAsync(dto.Email);

            if (customer == null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(
                customer,
                customer.PasswordHash,
                dto.Password);

            if (result == PasswordVerificationResult.Failed)
                return null;

            return _jwtService.GenerateToken(customer); 
        }
    }
}