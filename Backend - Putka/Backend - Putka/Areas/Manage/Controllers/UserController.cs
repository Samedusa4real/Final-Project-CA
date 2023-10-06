using Backend___Putka.DAL;
using Backend___Putka.Models;
using Backend___Putka.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Backend___Putka.Areas.Manage.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Area("manage")]
    public class UserController : Controller
    {
        private readonly PutkaDbContext _context;

        public UserController(PutkaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            var query = _context.Users.OrderBy(x => x.Id).AsQueryable();

            return View(PaginatedList<IdentityUser>.Create(query, page, 3));
        }

        public IActionResult Delete(string  id)
        {
            IdentityUser user = _context.Users.Find(id);

            if (user == null)
                return StatusCode(404);

            _context.Users.Remove(user);
            _context.SaveChanges();

            return StatusCode(200);
        }
    }
}
