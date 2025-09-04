using Microsoft.AspNetCore.Mvc;

namespace MyApp.Controllers
{
    public class PetsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
