using Backend___Putka.DAL;
using Microsoft.AspNetCore.Mvc;

namespace Backend___Putka.Areas.Manage.Controllers
{
    [Area("manage")]
    public class TagController : Controller
    {
        private readonly PutkaDbContext _context;

        public TagController(PutkaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
