using Backend___Putka.DAL;
using Backend___Putka.Models;
using Backend___Putka.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Backend___Putka.Areas.Manage.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    [Area("manage")]
    public class SettingController : Controller
    {
        private readonly PutkaDbContext _context;

        public SettingController(PutkaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            var query = _context.Settings.AsQueryable();

            return View(PaginatedList<Setting>.Create(query, page, 3));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Setting setting)
        {
            if (!ModelState.IsValid)
                return View();

            if (_context.Settings.Any(x => x.Key == setting.Key))
            {
                ModelState.AddModelError("Key", "This Key is already used!");
                return View();
            }

            _context.Settings.Add(setting);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(string key)
        {
            Setting setting = _context.Settings.Find(key);

            if (setting == null)
                return View("Error");

            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Setting setting)
        {
            if (!ModelState.IsValid)
                return View();

            Setting existedSetting = _context.Settings.Find(setting.Key);

            if (existedSetting == null)
                return View("Error");

            if (setting.Value != existedSetting.Value && _context.Settings.Any(x => x.Value == setting.Value))
            {
                ModelState.AddModelError("Value", "Value is already used!");
                return View();
            }

            existedSetting.Value = setting.Value;

            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(string key)
        {
            Setting setting = _context.Settings.Find(key);

            if (setting == null)
                return StatusCode(404);

            _context.Settings.Remove(setting);
            _context.SaveChanges();

            return StatusCode(200);
        }
    }
}
