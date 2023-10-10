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
    public class BannerController : Controller
    {
        private readonly PutkaDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public BannerController(PutkaDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public IActionResult Index(int page = 1)
        {
            var query = _context.Banners.OrderBy(x => x.Id).AsQueryable();

            return View(PaginatedList<Banner>.Create(query, page, 3));
        }
        
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Banner banner)
        {

            if (banner.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "ImageFile is required!");
                return View();
            }

            banner.BackgroundImage = FileManager.Save(_environment.WebRootPath, "uploads/banners", banner.ImageFile);

            _context.Banners.Add(banner);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            Banner banner = _context.Banners.Find(id);

            if (banner == null)
                return View("Error");

            return View(banner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Banner banner)
        {
            if (!ModelState.IsValid)
                return View();

            Banner existBanner = _context.Banners.Find(banner.Id);

            if (existBanner == null)
                return View("Error");

            existBanner.TopText = banner.TopText;
            existBanner.HeaderOne = banner.HeaderOne;
            existBanner.HeaderTwo = banner.HeaderTwo;
            existBanner.SaleText = banner.SaleText;
            existBanner.Description = banner.Description;
            existBanner.ButtonText = banner.ButtonText;
            existBanner.ButtonLink = banner.ButtonLink;
            existBanner.IsSmall = banner.IsSmall;

            string oldFileName = null;

            if (banner.ImageFile != null)
            {
                oldFileName = banner.BackgroundImage;
                existBanner.BackgroundImage = FileManager.Save(_environment.WebRootPath, "uploads/banners", banner.ImageFile);
            }

            _context.SaveChanges();

            if (oldFileName != null)
                FileManager.Save(_environment.WebRootPath, "uploads/banners", banner.ImageFile);

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Banner banner = _context.Banners.Find(id);

            if (banner == null)
                return StatusCode(404);

            _context.Banners.Remove(banner);
            _context.SaveChanges();

            return StatusCode(200);
        }
    }
}
