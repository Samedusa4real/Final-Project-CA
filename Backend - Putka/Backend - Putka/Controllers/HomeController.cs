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
                Products = _context.Products.Include(x => x.ProductImages).Where(x => x.StockStatus == true).Take(8).ToList(),
                BestSellerProducts = _context.Products.Include(x => x.ProductImages).Where(x => x.IsNew == false).Take(8).ToList(),
                TrendingProducts = _context.Products.Include(x=>x.ProductImages).OrderByDescending(x=>x.ProductComments.Any()).ThenByDescending(x=>x.ProductComments.Average(c=>c.Rate)).Take(4).ToList(),
                Categories = _context.Categories.ToList(),
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ContactUs(ContactUs contactUs)
        {
            if (!ModelState.IsValid)
                return View();

            if (contactUs.FirstName == null)
            {
                ModelState.AddModelError("FirstName", "FirstName is required!");
                return View();
            }

            if (contactUs.LastName == null)
            {
                ModelState.AddModelError("LastName", "LastName is required!");
                return View();
            }

            if (contactUs.Email == null)
            {
                ModelState.AddModelError("Email", "Email is required!");
                return View();
            }

            _context.ContactUs.Add(contactUs);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Blogs()
        {
            return View();
        }
    }
}