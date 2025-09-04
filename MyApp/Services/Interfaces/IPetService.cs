using MyApp.DTOs.Pets;

namespace MyApp.Services.Interfaces
{
    public interface IPetService
    {
        Task<PetDto?> GetByIdAsync(int id);
        Task<IEnumerable<PetDto>> GetAllAsync();
        Task<PetDto> CreateAsync(CreatePetDto dto);
        Task<PetDto?> UpdateAsync(int id, UpdatePetDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
