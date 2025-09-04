using Microsoft.AspNetCore.Mvc;

namespace MyApp.Controllers
{
    public class WishlistController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
