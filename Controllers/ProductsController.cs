using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StartupWebAPIs.Data;
using StartupWebAPIs.Helpers;
using StartupWebAPIs.Interfaces;
using StartupWebAPIs.Models;
using StartupWebAPIs.Responses;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace StartupWebAPIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        //private readonly AppDbContext _context;

        //public ProductsController(AppDbContext context)
        //{
        //    _context = context;
        //}
       // private readonly IProductRepository _repository;
        private readonly IProductService _service;

        //public ProductsController(IProductRepository repository)
        //{
        //    _repository = repository;
        //}

        public ProductsController(IProductService service)
        {
            _service = service;
        }
        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //   // var products = await _repository.GetAllAsync();
        //    return Ok(await _service.GetAllProductsAsync());
            
            
        //}

        [HttpGet]  //Pagination // Search
        public async Task<IActionResult> Get(string? search, int page = 1, int pageSize = 5)
        {
            var totalRecords = (await _service.GetAllProductsAsync()).Count();

                                                                                                   
            var products = (await _service.GetAllProductsAsync())
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new PagedResult<Product>
            {
                Data = products,
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };

            return Ok(result);

        }

        [HttpGet]  //Pagination
        public async Task<IActionResult> Get(string? search, int page = 1, int pageSize = 5)
        {
            var allProducts = await _service.GetAllProductsAsync();
            var totalRecords = allProducts.Count();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                allProducts = allProducts.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();
                totalRecords = allProducts.Count();
            }

            var products = allProducts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = new PagedResult<Product>
            {
                Data = products,
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            var created = await _service.CreateProductAsync(product);

            //return Ok(created);
            return Ok(new ApiResponse<Product>
            (
             true,
             "Product created successfully.",
             created
             ));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Product product)
        {
            var updated = await _service.UpdateProductAsync(id, product);

            if (updated == null)
                return NotFound();

            return Ok(new ApiResponse<Product>
             (
              true,
              "Product updated successfully.",
              updated
              ));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteProductAsync(id);

            if (!deleted)
                return NotFound();

            return Ok(new ApiResponse<string>
                (
                 true,
                 "Product deleted successfully.",
                 null
                 ));
        }
    }
}

