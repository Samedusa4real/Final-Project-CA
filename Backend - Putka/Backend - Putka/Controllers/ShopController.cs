using Microsoft.AspNetCore.Mvc;

namespace Backend___Putka.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Search()
        {
            return View();
        }
    }
}
