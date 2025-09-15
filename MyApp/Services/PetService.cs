using AutoMapper;
using MyApp.DTOs.Pets;
using MyApp.Entities;
using MyApp.Repositories.Interfaces;
using MyApp.Services.Interfaces;

namespace MyApp.Services
{
    public class PetService : IPetService
    {
        private readonly IPetRepository _petRepository;
        private readonly IMapper _mapper;

        public PetService(IPetRepository petRepository, IMapper mapper)
        {
            _petRepository = petRepository;
            _mapper = mapper;
        }

        public async Task<PetDto?> GetByIdAsync(int id)
        {
            var pet = await _petRepository.GetByIdAsync(id);
            return pet == null ? null : _mapper.Map<PetDto>(pet);
        }

        public async Task<IEnumerable<PetDto>> GetAllAsync()
        {
            var pets = await _petRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PetDto>>(pets);
        }

        public async Task<PetDto> CreateAsync(CreatePetDto dto, string? imageUrl)
        {
            var pet = _mapper.Map<Pet>(dto);

            if (!string.IsNullOrEmpty(imageUrl))
            {
                pet.ImageUrl = imageUrl;
            }

            await _petRepository.AddAsync(pet);
            await _petRepository.SaveChangesAsync();

            return _mapper.Map<PetDto>(pet);
        }

        public async Task<PetDto?> UpdateAsync(int id, UpdatePetDto dto, string? imageUrl)
        {
            var pet = await _petRepository.GetByIdAsync(id);
            if (pet == null) return null;

            _mapper.Map(dto, pet);

            if (!string.IsNullOrEmpty(imageUrl))
            {
                pet.ImageUrl = imageUrl;
            }

            await _petRepository.UpdateAsync(pet);
            await _petRepository.SaveChangesAsync();

            return _mapper.Map<PetDto>(pet);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pet = await _petRepository.GetByIdAsync(id);
            if (pet == null) return false;

            await _petRepository.DeleteAsync(pet);
            await _petRepository.SaveChangesAsync();
            return true;
        }
    }
}
