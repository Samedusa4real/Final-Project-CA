using Backend___Putka.DAL;
using Backend___Putka.Helpers;
using Backend___Putka.Models;
using Backend___Putka.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Backend___Putka.Areas.Manage.Controllers
{
    [Area("manage")]
    public class ProductController : Controller
    {
        private readonly PutkaDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductController(PutkaDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public IActionResult Index(int page = 1)
        {
            var query = _context.Products
                .Include(x => x.Category).Include(x => x.ProductImages.Where(pi => pi.IsMain == true)).AsQueryable();

            return View(PaginatedList<Product>.Create(query, page, 3));
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories
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
        public IActionResult Create(Product product)
        {
            if (!ModelState.IsValid)
                return View();

            if (product.CategoryId == 0)
            {
                ModelState.AddModelError("CategoryId", "Category is required");
                return View();
            }

            if (!_context.Categories.Any(x => x.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "CategoryId is not correct");
                return View();
            }

            if (product.PosterImage == null)
            {
                ModelState.AddModelError("PosterImage", "PosterImage is required");
                return View();
            }

            if (product.HoverImage == null)
            {
                ModelState.AddModelError("HoverImage", "HoverImage is required");
                return View();
            }

            foreach (var tagId in product.TagIds)
            {
                ProductTag productTag = new ProductTag
                {
                    TagId = tagId,
                    Product = product,
                };

                _context.ProductTags.Add(productTag);
            }

            ProductImage poster = new ProductImage
            {
                Url = FileManager.Save(_environment.WebRootPath, "uploads/products", product.PosterImage),
                IsMain = true,
                Product = product,
            };

            ProductImage hoverPoster = new ProductImage
            {
                Url = FileManager.Save(_environment.WebRootPath, "uploads/products", product.HoverImage),
                IsMain = false,
                Product = product,
            };

            _context.ProductImages.Add(poster);
            _context.ProductImages.Add(hoverPoster);

            _context.Products.Add(product);
            _context.SaveChanges();


            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Categories = _context.Categories
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

            Product product = _context.Products.Include(x => x.ProductImages).Include(x => x.ProductTags).FirstOrDefault(x => x.Id == id);

            product.TagIds = product.ProductTags.Select(x => x.TagId).ToList();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product)
        {
            if (!ModelState.IsValid)
                return View();

            Product existProduct = _context.Products.Include(x => x.ProductImages).Include(x => x.ProductTags).FirstOrDefault(x => x.Id == product.Id);

            if (existProduct == null)
                return View("Error");

            if (product.CategoryId != existProduct.CategoryId && !_context.Categories.Any(x => x.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "CategoryId is not correct!");
                return View();
            }

            existProduct.ProductTags.RemoveAll(x => !product.TagIds.Contains(x.TagId));

            var newTagIds = product.TagIds.FindAll(x => !existProduct.ProductTags.Any(y => y.TagId == x));
            foreach (var tagId in newTagIds)
            {
                ProductTag productTag = new ProductTag { TagId = tagId };
                existProduct.ProductTags.Add(productTag);
            }

            string oldPoster = null;
            if (product.PosterImage != null)
            {
                ProductImage poster = existProduct.ProductImages.FirstOrDefault(x => x.IsMain == true);
                oldPoster = poster?.Url;

                if (poster == null)
                {
                    poster = new ProductImage { IsMain = true };
                    poster.Url = FileManager.Save(_environment.WebRootPath, "uploads/products", product.PosterImage);
                    existProduct.ProductImages.Add(poster);
                }
                else
                    poster.Url = FileManager.Save(_environment.WebRootPath, "uploads/products", product.PosterImage);
            }

            string oldHoverPoster = null;
            if (product.PosterImage != null)
            {
                ProductImage poster = existProduct.ProductImages.FirstOrDefault(x => x.IsMain == false);
                oldHoverPoster = poster?.Url;

                if (poster == null)
                {
                    poster = new ProductImage { IsMain = false };
                    poster.Url = FileManager.Save(_environment.WebRootPath, "uploads/products", product.PosterImage);
                    existProduct.ProductImages.Add(poster);
                }
                else
                    poster.Url = FileManager.Save(_environment.WebRootPath, "uploads/products", product.PosterImage);
            }

            existProduct.Name = product.Name;
            existProduct.CategoryId = product.CategoryId;
            existProduct.Description = product.Description;
            existProduct.SalePrice = product.SalePrice;
            existProduct.CostPrice = product.CostPrice;
            existProduct.DiscountPercent = product.DiscountPercent;
            existProduct.IsNew = product.IsNew;
            existProduct.StockStatus = product.StockStatus;

            _context.SaveChanges();

            if (oldPoster != null) FileManager.Delete(_environment.WebRootPath, "uploads/products", oldPoster);
            if (oldHoverPoster != null) FileManager.Delete(_environment.WebRootPath, "uploads/products", oldHoverPoster);

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            Product product = _context.Products.Find(id);
            if (product == null)
                return StatusCode(404);

            _context.Products.Remove(product);
            _context.SaveChanges();

            return StatusCode(200);
        }
    }
}
