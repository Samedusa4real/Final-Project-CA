using Backend___Putka.DAL;
using Microsoft.AspNetCore.Mvc;

namespace Backend___Putka.ViewComponents
{
    public class FooterCategoriesViewComponent:ViewComponent
    {
        private readonly PutkaDbContext _context;

        public FooterCategoriesViewComponent(PutkaDbContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            var data = _context.Categories.ToList();
            return View(data);
        }
    }
}
