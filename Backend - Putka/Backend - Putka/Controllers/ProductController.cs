using Backend___Putka.DAL;
using Backend___Putka.Models;
using Backend___Putka.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Backend___Putka.Controllers
{
    public class ProductController : Controller
    {
        private readonly PutkaDbContext _context;

        public ProductController(PutkaDbContext context)
        {
            _context = context;
        }

        public IActionResult Detail(int id)
        {
            Product product = _context.Products
                .Include(x => x.ProductImages)
                .Include(x => x.Category)
                .Include(x=>x.ProductTags).ThenInclude(x=>x.Tag)
                .FirstOrDefault(x => x.Id == id);

            if (product == null) return View("Error");

            return PartialView("_QuickViewModalPartial", product);
        }

        public IActionResult AddToBasket(int id)
        {
            List<BasketItemCookieViewModel> cookieItems = new List<BasketItemCookieViewModel>();

            BasketItemCookieViewModel cookieItem;
            var basketStr = Request.Cookies["basket"];
            if (basketStr != null)
            {
                cookieItems = JsonConvert.DeserializeObject<List<BasketItemCookieViewModel>>(basketStr);

                cookieItem = cookieItems.FirstOrDefault(x => x.ProductId == id);

                if (cookieItem != null)
                    cookieItem.Count++;
                else
                {
                    cookieItem = new BasketItemCookieViewModel { ProductId = id, Count = 1 };
                    cookieItems.Add(cookieItem);
                }
            }
            else
            {
                cookieItem = new BasketItemCookieViewModel { ProductId = id, Count = 1 };
                cookieItems.Add(cookieItem);
            }

            Response.Cookies.Append("Basket", JsonConvert.SerializeObject(cookieItems));

            BasketViewModel bv = new BasketViewModel();
            foreach (var ci in cookieItems)
            {
                BasketItemViewModel bi = new BasketItemViewModel
                {
                    Count = ci.Count,
                    Product = _context.Products.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == ci.ProductId)
                };
                bv.BasketItems.Add(bi);
                bv.TotalPrice += (bi.Product.DiscountPercent > 0 ? (bi.Product.SalePrice * (100 - bi.Product.DiscountPercent) / 100) : bi.Product.SalePrice) * bi.Count;
            }

            return PartialView("_BasketPartialView", bv);
        }

        public IActionResult RemoveBasket(int id)
        {
            var basketStr = Request.Cookies["basket"];
            if (basketStr == null)
                return StatusCode(404);

            List<BasketItemCookieViewModel> cookieItems = JsonConvert.DeserializeObject<List<BasketItemCookieViewModel>>(basketStr);

            BasketItemCookieViewModel item = cookieItems.FirstOrDefault(x => x.ProductId == id);

            if (item == null)
                return StatusCode(404);

            if (item.Count > 1)
                item.Count--;
            else
                cookieItems.Remove(item);

            Response.Cookies.Append("basket", JsonConvert.SerializeObject(cookieItems));

            BasketViewModel bv = new BasketViewModel();
            foreach (var ci in cookieItems)
            {
                BasketItemViewModel bi = new BasketItemViewModel
                {
                    Count = ci.Count,
                    Product = _context.Products.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == ci.ProductId)
                };
                bv.BasketItems.Add(bi);
                bv.TotalPrice += (bi.Product.DiscountPercent > 0 ? (bi.Product.SalePrice * (100 - bi.Product.DiscountPercent) / 100) : bi.Product.SalePrice) * bi.Count;
            }

            return PartialView("_BasketPartialView", bv);
        }

        public IActionResult Cart()
        {
            return View();
        }

        public IActionResult Wishlist()
        {
            return View();
        }
    }
}
