using Microsoft.AspNetCore.Mvc;
using MyApp.DTOs.Common;
using MyApp.DTOs.Products;
using MyApp.Services.Interfaces;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // Test endpoint for exceptions
        [HttpGet("test-error")]
        public IActionResult TestError()
        {
            throw new Exception("Something went wrong in ProductsController!");
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetAll()
        {
            try
            {
                var products = await _productService.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(products, "Products fetched successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<ProductDto>>.FailResponse("Failed to fetch products", 500, new List<string> { ex.Message }));
            }
        }

        // GET: api/products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                if (product == null)
                    return NotFound(ApiResponse<ProductDto>.FailResponse("Product not found", 404));

                return Ok(ApiResponse<ProductDto>.SuccessResponse(product, "Product fetched successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<ProductDto>.FailResponse("Failed to fetch product", 500, new List<string> { ex.Message }));
            }
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProductDto>>> Create([FromBody] CreateProductDto dto)
        {
            try
            {
                var created = await _productService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id },
                    ApiResponse<ProductDto>.SuccessResponse(created, "Product created successfully", 201));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<ProductDto>.FailResponse("Failed to create product", 500, new List<string> { ex.Message }));
            }
        }

        // PUT: api/products/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> Update(int id, [FromBody] UpdateProductDto dto)
        {
            try
            {
                var updated = await _productService.UpdateAsync(id, dto);
                if (updated == null)
                    return NotFound(ApiResponse<ProductDto>.FailResponse("Product not found", 404));

                return Ok(ApiResponse<ProductDto>.SuccessResponse(updated, "Product updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<ProductDto>.FailResponse("Failed to update product", 500, new List<string> { ex.Message }));
            }
        }

        // DELETE: api/products/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                var success = await _productService.DeleteAsync(id);
                if (!success)
                    return NotFound(ApiResponse<object>.FailResponse("Product not found", 404));

                return Ok(ApiResponse<object>.SuccessResponse(null, "Product deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse("Failed to delete product", 500, new List<string> { ex.Message }));
            }
        }
    }
}
