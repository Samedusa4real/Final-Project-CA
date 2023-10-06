using Backend___Putka.DAL;
using Backend___Putka.Models;
using Backend___Putka.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Backend___Putka.Areas.Manage.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Area("manage")]
    public class RoleController : Controller
    {
        private readonly PutkaDbContext _context;

        public RoleController(PutkaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            var query = _context.Roles.OrderBy(x => x.Id).AsQueryable();

            return View(PaginatedList<IdentityRole>.Create(query, page, 3));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IdentityRole role)
        {
            if (!ModelState.IsValid)
                return View();

            if (_context.Roles.Any(x => x.Name == role.Name))
            {
                ModelState.AddModelError("Name", "This Role name is already used!");
                return View();
            }

            _context.Roles.Add(role);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
    
        public IActionResult Edit(string id)
        {
            IdentityRole role = _context.Roles.Find(id);

            if (role == null)
                return View("Error");

            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(IdentityRole role)
        {
            if (!ModelState.IsValid)
                return View();

            IdentityRole existedRole = _context.Roles.Find(role.Id);

            if (existedRole == null)
                return View("Error");

            if (role.Name != existedRole.Name && _context.Roles.Any(x => x.Name == role.Name))
            {
                ModelState.AddModelError("Name", "Role is already used!");
                return View();
            }

            existedRole.Name = role.Name;

            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(string id)
        {
            IdentityRole role = _context.Roles.Find(id);

            if (role == null)
                return StatusCode(404);

            _context.Roles.Remove(role);
            _context.SaveChanges();

            return StatusCode(200);
        }
    }
}
