using MyApp.DTOs.Products;

namespace MyApp.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto?> GetByIdAsync(int id);
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto> CreateAsync(CreateProductDto dto, string? imageUrl);
        Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto, string? imageUrl);
        Task<bool> DeleteAsync(int id);
    }
}
