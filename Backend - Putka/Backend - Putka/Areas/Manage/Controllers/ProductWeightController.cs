using Backend___Putka.DAL;
using Backend___Putka.Models;
using Backend___Putka.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Backend___Putka.Areas.Manage.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    [Area("manage")]
    public class ProductWeightController : Controller
    {
        private readonly PutkaDbContext _context;

        public ProductWeightController(PutkaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            var query = _context.ProductWeights.Include(x => x.Weight).Include(x => x.Product).AsQueryable();

            return View(PaginatedList<ProductWeight>.Create(query, page, 3));
        }
        public IActionResult Create()
        {
            ViewBag.Products = _context.Products
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                }).ToList();

            ViewBag.Weights = _context.Weights
                .Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.Name.ToString("0.00")
                }).ToList();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductWeight productWeight)
        {
            if (!ModelState.IsValid)
                return View();

            if (!_context.Products.Any(x => x.Id == productWeight.ProductId))
            {
                ModelState.AddModelError("ProductId", "Product is not correct");
                return View();
            }

            if (!_context.Weights.Any(x => x.Id == productWeight.WeightId))
            {
                ModelState.AddModelError("WeightId", "Weight(kg) is not correct");
                return View();
            }

            if (_context.ProductWeights.Any(pt => pt.ProductId == productWeight.ProductId && pt.WeightId == productWeight.WeightId))
            {
                ModelState.AddModelError("WeightId", "The relationship between this product and weight is already exist!");
                return View();
            }

            _context.ProductWeights.Add(productWeight);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(int productId, int weightId)
        {
            ViewBag.Products = _context.Products
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                }).ToList();

            ViewBag.Weights = _context.Weights
                .Select(w => new SelectListItem
                {
                    Value = w.Id.ToString(),
                    Text = w.Name.ToString("0.00")
                }).ToList();

            ProductWeight productWeight = _context.ProductWeights.FirstOrDefault(pt => pt.ProductId == productId && pt.WeightId == weightId);

            if (productWeight == null)
                return View("Error");

            return View(productWeight);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int productId, int weightId, ProductWeight productWeight)
        {
            if (!ModelState.IsValid)
                return View();

            ProductWeight existedProductWeight = _context.ProductWeights.FirstOrDefault(pt => pt.ProductId == productId && pt.WeightId == weightId);

            if (existedProductWeight == null)
                return View("Error");

            if (!_context.Products.Any(x => x.Id == productWeight.ProductId))
            {
                ModelState.AddModelError("ProductId", "Product is not correct");
                return View();
            }

            if (!_context.Weights.Any(x => x.Id == productWeight.WeightId))
            {
                ModelState.AddModelError("WeightId", "Weight(kg) is not correct");
                return View();
            }

            if (_context.ProductWeights.Any(pt => pt.ProductId == productWeight.ProductId && pt.WeightId == productWeight.WeightId))
            {
                ModelState.AddModelError("WeightId", "The relationship between this product and weight is already exist!");
                return View();
            }

            existedProductWeight.ProductId = productWeight.ProductId;
            existedProductWeight.WeightId = productWeight.WeightId;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int productId, int weightId)
        {
            ProductWeight productWeight = _context.ProductWeights.FirstOrDefault(pt => pt.ProductId == productId && pt.WeightId == weightId);

            if (productWeight == null)
                return StatusCode(404);

            _context.ProductWeights.Remove(productWeight);
            _context.SaveChanges();

            return StatusCode(200);
        }
    }
}
