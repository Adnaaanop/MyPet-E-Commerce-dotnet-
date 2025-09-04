using Microsoft.AspNetCore.Mvc;

namespace MyApp.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
