using Backend___Putka.DAL;
using Backend___Putka.Models;
using Backend___Putka.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Backend___Putka.Areas.Manage.Controllers
{
    [Area("manage")]
    public class WeightController : Controller
    {
        private readonly PutkaDbContext _context;

        public WeightController(PutkaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            var query = _context.Weights.OrderBy(x => x.Id).AsQueryable();

            return View(PaginatedList<Weight>.Create(query, page, 3));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Weight weight)
        {
            if (!ModelState.IsValid)
                return View();

            if (_context.Weights.Any(x => x.Name == weight.Name))
            {
                ModelState.AddModelError("Name", "This Weight is already used!");
                return View();
            }

            _context.Weights.Add(weight);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            Weight weight = _context.Weights.Find(id);

            if (weight == null)
                return View("Error");

            return View(weight);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Weight weight)
        {
            if (!ModelState.IsValid)
                return View();

            Weight existedWeight = _context.Weights.Find(weight.Id);

            if (existedWeight == null)
                return View("Error");

            if (weight.Name != existedWeight.Name && _context.Weights.Any(x => x.Name == weight.Name))
            {
                ModelState.AddModelError("Name", "Weight(kg) is already used!");
                return View();
            }

            existedWeight.Name = weight.Name;

            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Weight weight = _context.Weights.Find(id);

            if (weight == null)
                return StatusCode(404);

            _context.Weights.Remove(weight);
            _context.SaveChanges();

            return StatusCode(200);
        }
    }
}
