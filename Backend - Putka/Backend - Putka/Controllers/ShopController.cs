using Backend___Putka.DAL;
using Backend___Putka.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Backend___Putka.Controllers
{
    public class ShopController : Controller
    {
        private readonly PutkaDbContext _context;

        public ShopController(PutkaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(List<int> categoryId, List<int> tagId, int page = 1, int pageSize = 6, double? minPrice = null, double? maxPrice = null, string sort = "Latest")
        {
            ShopViewModel shopVM = new ShopViewModel
            {
                Categories = _context.Categories.Include(x=>x.Products).ToList(),

                Weights = _context.Weights.ToList(),

                Tags = _context.Tags.ToList()
            };

            var query = _context.Products.Include(x => x.Category)
                                            .Include(x => x.ProductImages)
                                            .Include(x => x.ProductTags)
                                            .Include(x => x.ProductWeights)
                                            .AsQueryable();

            if (categoryId.Count > 0)
                query = query.Where(x => categoryId.Contains(x.CategoryId));

            if (tagId.Count > 0)
                query = query.Where(x => tagId.Contains(x.ProductTags.FirstOrDefault(y=>y.ProductId == x.Id).TagId));

            if (minPrice != null && maxPrice != null)
                query = query.Where(x => x.SalePrice >= (decimal)minPrice && x.SalePrice <= (decimal)maxPrice);

            switch (sort)
            {
                case "AtoZ":
                    query = query.OrderBy(x => x.Name);
                    break;
                case "ZtoA":
                    query = query.OrderByDescending(x => x.Name);
                    break;
                case "LowToHigh":
                    query = query.OrderBy(x => x.SalePrice);
                    break;
                case "HighToLow":
                    query = query.OrderByDescending(x => x.SalePrice);
                    break;
                case "Latest":
                    query = query.OrderBy(x => x.IsNew);
                    break;
            }

            ViewBag.SortList = new List<SelectListItem>
            {
                new SelectListItem {Value="AtoZ",Text= "A-Z",Selected=sort=="AtoZ"},
                new SelectListItem { Value = "ZtoA", Text = "Z-A", Selected = sort == "ZtoA" },
                new SelectListItem { Value = "LowToHigh", Text = "Lowest Price", Selected = sort == "LowToHigh" },
                new SelectListItem { Value = "HighToLow", Text = "Highest Price", Selected = sort == "HighToLow" },
                new SelectListItem { Value = "HighToLow", Text = "Latest Product", Selected = sort == "Latest" }

            };

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var products = query.Skip((page - 1) * pageSize)
                              .Take(pageSize)
                              .ToList();


            shopVM.Products = query.ToList();

            ViewBag.MaxPriceLimit = _context.Products.Max(x => x.SalePrice);

            ViewBag.Count = products.Count;
            ViewBag.AllProducts = _context.Products.ToList();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;

            ViewBag.Sort = sort;
            ViewBag.CategoryIds = categoryId;
            ViewBag.TagIds = tagId;
            ViewBag.MinPrice = minPrice ?? 0;
            ViewBag.MaxPrice = maxPrice ?? ViewBag.MaxPriceLimit;

            return View(shopVM);
        }

        public IActionResult Search()
        {
            return View();
        }
    }
}
