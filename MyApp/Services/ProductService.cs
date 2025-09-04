using MyApp.DTOs.Products;
using MyApp.Entities;
using MyApp.Repositories.Interfaces;
using MyApp.Services.Interfaces;

namespace MyApp.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product == null ? null : MapToDto(product);
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(MapToDto).ToList();
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Category = dto.Category,
                Price = dto.Price,
                Description = dto.Description,
                Rating = dto.Rating,
                Stock = dto.Stock,
                ImageUrl = dto.ImageUrl
            };

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            product.Name = dto.Name;
            product.Category = dto.Category;
            product.Price = dto.Price;
            product.Description = dto.Description;
            product.Rating = dto.Rating;
            product.Stock = dto.Stock;
            product.ImageUrl = dto.ImageUrl;

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            return MapToDto(product);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return false;

            await _productRepository.DeleteAsync(product);
            await _productRepository.SaveChangesAsync();
            return true;
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,
                Rating = product.Rating,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl
            };
        }
    }
}
