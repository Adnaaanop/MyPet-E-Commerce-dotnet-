using Microsoft.AspNetCore.Mvc;

namespace MyApp.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
