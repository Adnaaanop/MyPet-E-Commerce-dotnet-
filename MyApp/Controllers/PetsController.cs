using Microsoft.AspNetCore.Mvc;
using MyApp.DTOs.Pets;
using MyApp.DTOs.Common;
using MyApp.Services.Interfaces;
using MyApp.Services;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetsController : ControllerBase
    {
        private readonly IPetService _petService;
        private readonly CloudinaryService _cloudinaryService;

        public PetsController(IPetService petService, CloudinaryService cloudinaryService)
        {
            _petService = petService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<PetDto>>>> GetAll(
            [FromQuery] string? category,
            [FromQuery] string? search,
            [FromQuery] string? sortOrder,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 0)
        {
            try
            {
                var pets = await _petService.GetAllFilteredAsync(category, search, sortOrder);

                // ✅ Pagination (optional)
                if (pageSize > 0)
                {
                    pets = pets.Skip((page - 1) * pageSize).Take(pageSize);
                }

                return Ok(ApiResponse<IEnumerable<PetDto>>.SuccessResponse(pets, "Pets fetched successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<PetDto>>.FailResponse("Failed to fetch pets", 500, new List<string> { ex.Message }));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<PetDto>>> GetById(int id)
        {
            try
            {
                var pet = await _petService.GetByIdAsync(id);
                if (pet == null)
                    return NotFound(ApiResponse<PetDto>.FailResponse("Pet not found", 404));

                return Ok(ApiResponse<PetDto>.SuccessResponse(pet, "Pet fetched successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PetDto>.FailResponse("Failed to fetch pet", 500, new List<string> { ex.Message }));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PetDto>>> Create([FromForm] CreatePetDto dto)
        {
            try
            {
                string? imageUrl = null;
                if (dto.ImageFile != null)
                {
                    imageUrl = await _cloudinaryService.UploadImageAsync(dto.ImageFile);
                }

                var created = await _petService.CreateAsync(dto, imageUrl);
                return CreatedAtAction(nameof(GetById), new { id = created.Id },
                    ApiResponse<PetDto>.SuccessResponse(created, "Pet created successfully", 201));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PetDto>.FailResponse("Failed to create pet", 500, new List<string> { ex.Message }));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<PetDto>>> Update(int id, [FromForm] UpdatePetDto dto)
        {
            try
            {
                string? imageUrl = null;
                if (dto.ImageFile != null)
                {
                    imageUrl = await _cloudinaryService.UploadImageAsync(dto.ImageFile);
                }

                var updated = await _petService.UpdateAsync(id, dto, imageUrl);
                if (updated == null)
                    return NotFound(ApiResponse<PetDto>.FailResponse("Pet not found", 404));

                return Ok(ApiResponse<PetDto>.SuccessResponse(updated, "Pet updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PetDto>.FailResponse("Failed to update pet", 500, new List<string> { ex.Message }));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                var success = await _petService.DeleteAsync(id);
                if (!success)
                    return NotFound(ApiResponse<object>.FailResponse("Pet not found", 404));

                return Ok(ApiResponse<object>.SuccessResponse(null, "Pet deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailResponse("Failed to delete pet", 500, new List<string> { ex.Message }));
            }
        }
    }
}
