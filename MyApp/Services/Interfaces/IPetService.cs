using MyApp.DTOs.Pets;

namespace MyApp.Services.Interfaces
{
    public interface IPetService
    {
        Task<PetDto?> GetByIdAsync(int id);
        Task<IEnumerable<PetDto>> GetAllAsync();

        // ✅ NEW: Get pets with optional filtering, searching, and sorting
        Task<IEnumerable<PetDto>> GetAllFilteredAsync(string? category, string? search, string? sortOrder);

        Task<PetDto> CreateAsync(CreatePetDto dto, string? imageUrl);
        Task<PetDto?> UpdateAsync(int id, UpdatePetDto dto, string? imageUrl);
        Task<bool> DeleteAsync(int id);
    }
}
