using Backend___Putka.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend___Putka.ViewComponents
{
    public class NavCategoriesViewComponent:ViewComponent
    {
        private readonly PutkaDbContext _context;

        public NavCategoriesViewComponent(PutkaDbContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            var data = _context.Categories.Include(x=>x.Products).ToList();
            return View(data);
        }
    }
}
