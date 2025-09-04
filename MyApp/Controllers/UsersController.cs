using Microsoft.AspNetCore.Mvc;

namespace MyApp.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
