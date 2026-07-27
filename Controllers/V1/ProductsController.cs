using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StartupWebAPIs.Data;
using StartupWebAPIs.DTOs;
using StartupWebAPIs.Helpers;
using StartupWebAPIs.Interfaces;
using StartupWebAPIs.Models;
using StartupWebAPIs.Responses;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace StartupWebAPIs.Controllers.V1
{
    [ApiController]
    //[Route("api/[controller]")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductsController : ControllerBase
    {
        //private readonly AppDbContext _context;

        //public ProductsController(AppDbContext context)
        //{
        //    _context = context;
        //}
        // private readonly IProductRepository _repository;
        private readonly IProductService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductsController> _logger;
        //public ProductsController(IProductRepository repository)
        //{
        //    _repository = repository;
        //}

        public ProductsController(IProductService service, IMapper mapper, ILogger<ProductsController> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }
        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //   // var products = await _repository.GetAllAsync();
        //    return Ok(await _service.GetAllProductsAsync());


        //}

        [HttpGet]  //Pagination // Search
        //public async Task<IActionResult> Get(string? sort, string? search, int page = 1, int pageSize = 5)
        public async Task<IActionResult> Get(string? search, string? sort, decimal? minPrice, decimal? maxPrice, int page = 1, int pageSize = 5)
        {
            // var products = await _service.GetAllProductsAsync();
            //var products = await _service.GetAllProductsAsync(search, sort);
            _logger.LogInformation("Getting products. Search={Search}, Sort={Sort}, MinPrice={MinPrice}, MaxPrice={MaxPrice}, Page={Page}, PageSize={PageSize}", search, sort, minPrice, maxPrice, page, pageSize);
            var products = await _service.GetAllProductsAsync(search, sort, minPrice, maxPrice);
            //  Sort started
            if (!string.IsNullOrWhiteSpace(sort))
            {
                switch (sort.ToLower())
                {
                    case "name":
                        products = products.OrderBy(x => x.Name).ToList();
                        break;

                    case "name_desc":
                        products = products.OrderByDescending(x => x.Name).ToList();
                        break;

                    case "price":
                        products = products.OrderBy(x => x.Price).ToList();
                        break;

                    case "price_desc":
                        products = products.OrderByDescending(x => x.Price).ToList();
                        break;
                }
            }
            //Sort End
            var totalRecords = products.Count();

            var pagedProducts = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var productDtos = _mapper.Map<List<ProductDto>>(pagedProducts);

            var result = new PagedResult<ProductDto>
            {
                Data = productDtos,
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };

            return Ok(result);
        }



        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {

            var product = _mapper.Map<Product>(dto);
            _logger.LogInformation("Creating product {Name} with Price={Price}", product.Name, product.Price);
            var created = await _service.CreateProductAsync(product);

            var result = _mapper.Map<ProductDto>(created);
            _logger.LogInformation("Product created successfully. Id={Id}", created.Id);
            return Ok(new ApiResponse<ProductDto>
            (
                true,
                "Product created successfully.",
                result
            ));


        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);

            var updated = await _service.UpdateProductAsync(id, product);
            _logger.LogInformation("Updating Product Id={Id}", id);
            _logger.LogWarning("Product Id={Id} not found", id);
            if (updated == null)
                return NotFound();

            var result = _mapper.Map<ProductDto>(updated);

            return Ok(new ApiResponse<ProductDto>
            (
                true,
                "Product updated successfully.",
                result
            ));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteProductAsync(id);
            _logger.LogInformation("Deleting Product Id={Id}", id);
            _logger.LogWarning("Delete failed. Product Id={Id} not found", id);
            if (!deleted)
                return NotFound();
            _logger.LogInformation("Product Id={Id} deleted successfully", id);
            return Ok(new ApiResponse<string>
                (
                 true,
                 "Product deleted successfully.",
                 null
                 ));
        }
    }
}

