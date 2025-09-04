using Microsoft.AspNetCore.Mvc;

namespace MyApp.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
