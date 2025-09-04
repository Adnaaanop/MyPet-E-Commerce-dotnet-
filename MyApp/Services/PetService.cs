using MyApp.DTOs.Pets;
using MyApp.Entities;
using MyApp.Repositories.Interfaces;
using MyApp.Services.Interfaces;

namespace MyApp.Services
{
    public class PetService : IPetService
    {
        private readonly IPetRepository _petRepository;

        public PetService(IPetRepository petRepository)
        {
            _petRepository = petRepository;
        }

        public async Task<PetDto?> GetByIdAsync(int id)
        {
            var pet = await _petRepository.GetByIdAsync(id);
            return pet == null ? null : MapToDto(pet);
        }

        public async Task<IEnumerable<PetDto>> GetAllAsync()
        {
            var pets = await _petRepository.GetAllAsync();
            return pets.Select(MapToDto).ToList();
        }

        public async Task<PetDto> CreateAsync(CreatePetDto dto)
        {
            var pet = new Pet
            {
                Name = dto.Name,
                Breed = dto.Breed,
                Age = dto.Age,
                Price = dto.Price,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl
            };

            await _petRepository.AddAsync(pet);
            await _petRepository.SaveChangesAsync();

            return MapToDto(pet);
        }

        public async Task<PetDto?> UpdateAsync(int id, UpdatePetDto dto)
        {
            var pet = await _petRepository.GetByIdAsync(id);
            if (pet == null) return null;

            pet.Name = dto.Name;
            pet.Breed = dto.Breed;
            pet.Age = dto.Age;
            pet.Price = dto.Price;
            pet.Description = dto.Description;
            pet.ImageUrl = dto.ImageUrl;

            await _petRepository.UpdateAsync(pet);
            await _petRepository.SaveChangesAsync();

            return MapToDto(pet);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pet = await _petRepository.GetByIdAsync(id);
            if (pet == null) return false;

            await _petRepository.DeleteAsync(pet);
            await _petRepository.SaveChangesAsync();
            return true;
        }

        private static PetDto MapToDto(Pet pet)
        {
            return new PetDto
            {
                Id = pet.Id,
                Name = pet.Name,
                Breed = pet.Breed,
                Age = pet.Age,
                Price = pet.Price,
                Description = pet.Description,
                ImageUrl = pet.ImageUrl
            };
        }
    }
}
