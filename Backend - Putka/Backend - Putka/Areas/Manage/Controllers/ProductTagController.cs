using Backend___Putka.DAL;
using Backend___Putka.Models;
using Backend___Putka.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
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
            ViewBag.Products = _context.Products
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                }).ToList();

            ViewBag.Tags = _context.Tags
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name
                }).ToList();

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

            if (_context.ProductTags.Any(pt => pt.ProductId == productTag.ProductId && pt.TagId == productTag.TagId))
            {
                ModelState.AddModelError("", "The relationship between this product and tag is already exist!");
                return View();
            }

            _context.ProductTags.Add(productTag);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(int productId, int tagId)
        {
            ViewBag.Products = _context.Products
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                }).ToList();

            ViewBag.Tags = _context.Tags
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name
                }).ToList();

            ProductTag productTag = _context.ProductTags.FirstOrDefault(pt => pt.ProductId == productId && pt.TagId == tagId);

            if (productTag == null)
                return View("Error");

            return View(productTag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int productId, int tagId, ProductTag productTag)
        {
            if (!ModelState.IsValid)
                return View();

            ProductTag existedProductTag = _context.ProductTags.FirstOrDefault(pt => pt.ProductId == productId && pt.TagId == tagId);

            if (existedProductTag == null)
                return View("Error");

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

            if (_context.ProductTags.Any(pt => pt.ProductId == productTag.ProductId && pt.TagId == productTag.TagId))
            {
                ModelState.AddModelError("", "The relationship between this product and tag is already exist!");
                return View();
            }

            existedProductTag.ProductId = productTag.ProductId;
            existedProductTag.TagId = productTag.TagId;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int productId, int tagId)
        {
            ProductTag productTag = _context.ProductTags.FirstOrDefault(pt => pt.ProductId == productId && pt.TagId == tagId);

            if (productTag == null)
                return StatusCode(404);

            _context.ProductTags.Remove(productTag);
            _context.SaveChanges();

            return StatusCode(200);
        }
    }
}
