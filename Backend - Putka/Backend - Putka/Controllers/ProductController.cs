using Backend___Putka.DAL;
using Backend___Putka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend___Putka.Controllers
{
    public class ProductController : Controller
    {
        private readonly PutkaDbContext _context;

        public ProductController(PutkaDbContext context)
        {
            _context = context;
        }

        public IActionResult Detail(int id)
        {
            Product product = _context.Products
                .Include(x => x.ProductImages)
                .Include(x => x.Category)
                .Include(x=>x.ProductTags).ThenInclude(x=>x.Tag)
                .FirstOrDefault(x => x.Id == id);

            if (product == null) return View("Error");

            return PartialView("_QuickViewModalPartial", product);
        }

        public IActionResult Cart()
        {
            return View();
        }

        public IActionResult Wishlist()
        {
            return View();
        }
    }
}
