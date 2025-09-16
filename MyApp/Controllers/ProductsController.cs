using Microsoft.AspNetCore.Mvc;
using MyApp.DTOs.Common;
using MyApp.DTOs.Products;
using MyApp.Services.Interfaces;
using MyApp.Services;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly CloudinaryService _cloudinaryService;

        public ProductsController(IProductService productService, CloudinaryService cloudinaryService)
        {
            _productService = productService;
            _cloudinaryService = cloudinaryService;
        }

        // GET: api/products?category=&search=&sortOrder=&page=&pageSize=
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetAll(
            [FromQuery] string? category,
            [FromQuery] string? search,
            [FromQuery] string? sortOrder,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 0)
        {
            try
            {
                var products = await _productService.GetAllFilteredAsync(category, search, sortOrder);

                // Pagination
                if (pageSize > 0)
                {
                    products = products.Skip((page - 1) * pageSize).Take(pageSize);
                }

                return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(products, "Products fetched successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<ProductDto>>.FailResponse(
                    "Failed to fetch products", 500, new List<string> { ex.Message }));
            }
        }

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
                return StatusCode(500, ApiResponse<ProductDto>.FailResponse(
                    "Failed to fetch product", 500, new List<string> { ex.Message }));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProductDto>>> Create([FromForm] CreateProductDto dto)
        {
            try
            {
                string? imageUrl = null;
                if (dto.ImageFile != null)
                    imageUrl = await _cloudinaryService.UploadImageAsync(dto.ImageFile);

                var created = await _productService.CreateAsync(dto, imageUrl);
                return CreatedAtAction(nameof(GetById), new { id = created.Id },
                    ApiResponse<ProductDto>.SuccessResponse(created, "Product created successfully", 201));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<ProductDto>.FailResponse(
                    "Failed to create product", 500, new List<string> { ex.Message }));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ProductDto>>> Update(int id, [FromForm] UpdateProductDto dto)
        {
            try
            {
                string? imageUrl = null;
                if (dto.ImageFile != null)
                    imageUrl = await _cloudinaryService.UploadImageAsync(dto.ImageFile);

                var updated = await _productService.UpdateAsync(id, dto, imageUrl);
                if (updated == null)
                    return NotFound(ApiResponse<ProductDto>.FailResponse("Product not found", 404));

                return Ok(ApiResponse<ProductDto>.SuccessResponse(updated, "Product updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<ProductDto>.FailResponse(
                    "Failed to update product", 500, new List<string> { ex.Message }));
            }
        }

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
                return StatusCode(500, ApiResponse<object>.FailResponse(
                    "Failed to delete product", 500, new List<string> { ex.Message }));
            }
        }
    }
}
