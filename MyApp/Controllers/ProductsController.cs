using Microsoft.AspNetCore.Mvc;
using MyApp.Data;

namespace MyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProductsController(AppDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetProducts()
        {
            return Ok(_context.Products.ToList());
        }
    }
}