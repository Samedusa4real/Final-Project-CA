using Backend___Putka.DAL;
using Backend___Putka.Models;
using Backend___Putka.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend___Putka.Areas.Manage.Controllers
{
    [Area("manage")]
    public class ProductTagController : Controller
    {
        private readonly PutkaDbContext _context;

        public ProductTagController(PutkaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            var query = _context.ProductTags.Include(x=>x.Tag).Include(x=>x.Product).AsQueryable();

            return View(PaginatedList<ProductTag>.Create(query, page, 3));
        }

        public IActionResult Create()
        {
            ViewBag.Products = _context.Products.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductTag productTag)
        {
            if (!ModelState.IsValid)
                return View();

            if (!_context.Products.Any(x => x.Id == productTag.ProductId))
            {
                ModelState.AddModelError("ProductId", "Product is not correct");
                return View();
            }

            if (!_context.Tags.Any(x => x.Id == productTag.TagId))
            {
                ModelState.AddModelError("TagId", "Tag is not correct");
                return View();
            }

            _context.ProductTags.Add(productTag);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
    }
}
