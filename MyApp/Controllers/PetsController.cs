using Microsoft.AspNetCore.Mvc;
using MyApp.DTOs.Pets;
using MyApp.Services.Interfaces;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetsController : ControllerBase
    {
        private readonly IPetService _petService;

        public PetsController(IPetService petService)
        {
            _petService = petService;
        }

        // GET: api/pets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PetDto>>> GetAll()
        {
            var pets = await _petService.GetAllAsync();
            return Ok(pets);
        }

        // GET: api/pets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PetDto>> GetById(int id)
        {
            var pet = await _petService.GetByIdAsync(id);
            if (pet == null) return NotFound();
            return Ok(pet);
        }

        // POST: api/pets
        [HttpPost]
        public async Task<ActionResult<PetDto>> Create([FromBody] CreatePetDto dto)
        {
            var created = await _petService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/pets/5
        [HttpPut("{id}")]
        public async Task<ActionResult<PetDto>> Update(int id, [FromBody] UpdatePetDto dto)
        {
            var updated = await _petService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // DELETE: api/pets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _petService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
