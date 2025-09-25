using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace MyApp.DTOs.Pets
{
    public class CreatePetDto
    {
        public string Name { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public int Age { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Stock { get; set; }
        public string? ImageUrl { get; set; } // Added to support URL input
        public IFormFile? ImageFile { get; set; }
    }
}