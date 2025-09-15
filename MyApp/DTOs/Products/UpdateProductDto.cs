using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace MyApp.DTOs.Products
{
    public class UpdateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public double Rating { get; set; }
        public int Stock { get; set; }

        // ✅ Only file upload

        // File upload
        public IFormFile? ImageFile { get; set; }
    }
}
