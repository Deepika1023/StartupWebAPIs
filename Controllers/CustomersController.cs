using Microsoft.AspNetCore.Mvc;
using StartupWebAPIs.DTOs.Customers;
using StartupWebAPIs.Interfaces;
using StartupWebAPIs.Models;
using StartupWebAPIs.Responses;

namespace StartupWebAPIs.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _service;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(
            ICustomerService service,
            ILogger<CustomersController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: api/customers
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _service.GetAllAsync();

            var result = customers.Select(customer => new CustomerDto
            {
                Id = customer.Id,
                CompanyName = customer.CompanyName,
                ContactPerson = customer.ContactPerson,
                Email = customer.Email,
                IsActive = customer.IsActive,
                SubscriptionPlan = customer.SubscriptionPlan?.Name ?? ""
            }).ToList();

            return Ok(new ApiResponse<List<CustomerDto>>
            (
                true,
                "Customers retrieved successfully.",
                result
            ));
        }

        // GET: api/customers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var customer = await _service.GetByIdAsync(id);

            if (customer == null)
                return NotFound(new ApiResponse<string>
                (
                    false,
                    "Customer not found.",
                    null
                ));

            var result = new CustomerDto
            {
                Id = customer.Id,
                CompanyName = customer.CompanyName,
                ContactPerson = customer.ContactPerson,
                Email = customer.Email,
                IsActive = customer.IsActive,
                SubscriptionPlan = customer.SubscriptionPlan?.Name ?? ""
            };

            return Ok(new ApiResponse<CustomerDto>
            (
                true,
                "Customer retrieved successfully.",
                result
            ));
        }

        // POST: api/customers/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCustomerDto dto)
        {
            var customer = await _service.RegisterAsync(dto);

            _logger.LogInformation(
                "Customer {Email} registered successfully.",
                customer.Email);

            var result = new CustomerDto
            {
                Id = customer.Id,
                CompanyName = customer.CompanyName,
                ContactPerson = customer.ContactPerson,
                Email = customer.Email,
                IsActive = customer.IsActive,
                SubscriptionPlan = customer.SubscriptionPlan?.Name ?? "Free"
            };

            return Ok(new ApiResponse<CustomerDto>
            (
                true,
                "Customer registered successfully.",
                result
            ));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCustomerDto dto)
        {
            var token = await _service.LoginAsync(dto);

            if (token == null)
            {
                return Unauthorized(new ApiResponse<string>
                (
                    false,
                    "Invalid email or password.",
                    null
                ));
            }

            _logger.LogInformation("Customer {Email} logged in successfully.", dto.Email);

            return Ok(new ApiResponse<string>
            (
                true,
                "Login successful.",
                token
            ));
        }
        // DELETE: api/customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new ApiResponse<string>
                (
                    false,
                    "Customer not found.",
                    null
                ));
            }

            return Ok(new ApiResponse<string>
            (
                true,
                "Customer deleted successfully.",
                null
            ));
        }
    }
}