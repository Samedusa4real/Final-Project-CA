using Backend___Putka.DAL;
using Backend___Putka.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Backend___Putka.Controllers
{
    public class HomeController : Controller
    {
        private readonly PutkaDbContext _context;

        public HomeController(PutkaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}