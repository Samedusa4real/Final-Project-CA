using Backend___Putka.DAL;
using Backend___Putka.Helpers;
using Backend___Putka.Models;
using Backend___Putka.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Backend___Putka.Areas.Manage.Controllers
{
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
    }
}
