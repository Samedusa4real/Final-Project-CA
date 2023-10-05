using Backend___Putka.DAL;
using Backend___Putka.Enums;
using Backend___Putka.Models;
using Backend___Putka.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend___Putka.Areas.Manage.Controllers
{
    [Area("manage")]
    public class OrderController : Controller
    {
        private readonly PutkaDbContext _context;

        public OrderController(PutkaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page = 1)
        {
            var query = _context.Orders.Include(x => x.OrderItems).ThenInclude(x => x.Product).AsQueryable();

            var data = PaginatedList<Order>.Create(query, page, 4);
            return View(data);
        }

        public IActionResult Detail(int id)
        {
            Order order = _context.Orders.Include(x => x.OrderItems).ThenInclude(x => x.Product).ThenInclude(x=>x.ProductImages).FirstOrDefault(x => x.Id == id);

            if (order == null)
                return View("Error");

            return View(order);
        }

        public IActionResult Accept(int id)
        {
            Order order = _context.Orders.FirstOrDefault(x => x.Id == id);

            if (order == null)
                return View("Error");

            order.Status = OrderStatus.Accepted;
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Reject(int id)
        {
            Order order = _context.Orders.FirstOrDefault(x => x.Id == id);

            if (order == null)
                return View("Error");

            order.Status = OrderStatus.Rejected;
            _context.SaveChanges();

            return RedirectToAction("index");
        }
    }
}
