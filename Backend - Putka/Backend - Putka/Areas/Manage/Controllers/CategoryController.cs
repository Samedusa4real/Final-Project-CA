using Backend___Putka.DAL;
using Backend___Putka.Helpers;
using Backend___Putka.Models;
using Backend___Putka.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Backend___Putka.Areas.Manage.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    [Area("manage")]
    public class CategoryController : Controller
    {
        private readonly PutkaDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CategoryController(PutkaDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public IActionResult Index(int page = 1)
        {
            var query = _context.Categories.OrderBy(x => x.Id).AsQueryable();

            return View(PaginatedList<Category>.Create(query, page, 3));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
                return View();

            if (category.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "ImageFile is required!");
                return View();
            }

            category.Icon = FileManager.Save(_environment.WebRootPath, "uploads/categories", category.ImageFile);

            _context.Categories.Add(category);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            Category category = _context.Categories.Find(id);

            if (category == null)
                return View("Error");

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
                return View();

            Category existCategory = _context.Categories.Find(category.Id);

            if (existCategory == null)
                return View("Error");

            existCategory.Name = category.Name;

            string oldFileName = null;

            if (category.ImageFile != null)
            {
                oldFileName = category.Icon;
                existCategory.Icon = FileManager.Save(_environment.WebRootPath, "uploads/categories", category.ImageFile);
            }

            _context.SaveChanges();

            if (oldFileName != null)
                FileManager.Save(_environment.WebRootPath, "uploads/categories", category.ImageFile);

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Category category = _context.Categories.Find(id);

            if (category == null)
                return StatusCode(404);

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return StatusCode(200);
        }
    }
}
