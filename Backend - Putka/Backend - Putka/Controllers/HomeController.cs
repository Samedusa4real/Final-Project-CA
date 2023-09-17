using Backend___Putka.DAL;
using Backend___Putka.Models;
using Backend___Putka.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            HomeViewModel hvm = new HomeViewModel
            {
                Products = _context.Products.Include(x=>x.ProductImages).Where(x => x.StockStatus == true).Take(8).ToList(),
                BestSellerProducts = _context.Products.Include(x => x.ProductImages).Where(x => x.IsNew == false).Take(8).ToList(),
            };

            return View(hvm);
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }

        public IActionResult Blogs()
        {
            return View();
        }
    }
}