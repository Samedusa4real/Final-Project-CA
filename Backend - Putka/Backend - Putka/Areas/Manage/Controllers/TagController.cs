using Backend___Putka.DAL;
using Backend___Putka.Models;
using Backend___Putka.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Security.Policy;

namespace Backend___Putka.Areas.Manage.Controllers
{
    [Area("manage")]
    public class TagController : Controller
    {
        private readonly PutkaDbContext _context;

        public TagController(PutkaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            var query = _context.Tags.OrderBy(x => x.Id).AsQueryable();

            return View(PaginatedList<Tag>.Create(query, page, 3));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Tag tag)
        {
            if (!ModelState.IsValid)
                return View();

            if (_context.Tags.Any(x => x.Name == tag.Name))
            {
                ModelState.AddModelError("Name", "This Tag name is already used!");
                return View();
            }

            _context.Tags.Add(tag);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            Tag tag = _context.Tags.Find(id);

            if (tag == null) 
                return View("Error");

            return View(tag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Tag tag)
        {
            if (!ModelState.IsValid)
                return View();

            Tag existedTag = _context.Tags.Find(tag.Id);

            if (existedTag == null)
                return View("Error");

            if (tag.Name != existedTag.Name && _context.Tags.Any(x => x.Name == tag.Name))
            {
                ModelState.AddModelError("Name", "Tag name is already used!");
                return View();
            }

            existedTag.Name = tag.Name;

            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Tag tag = _context.Tags.Find(id);

            if (tag == null)
                return StatusCode(404);

            _context.Tags.Remove(tag);
            _context.SaveChanges();

            return StatusCode(200);
        }
    }
}
