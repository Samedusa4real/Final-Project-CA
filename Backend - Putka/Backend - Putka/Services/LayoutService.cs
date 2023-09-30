using Backend___Putka.DAL;
using Backend___Putka.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Backend___Putka.Services
{
    public class LayoutService
    {
        private readonly PutkaDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LayoutService(PutkaDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public Dictionary<string, string> GetSettings()
        {
            return _context.Settings.ToDictionary(x => x.Key, x => x.Value);
        }

        public BasketViewModel GetBasket()
        {
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated && _httpContextAccessor.HttpContext.User.IsInRole("Member"))
            {
                var basketItems = _context.BasketItems.Include(x => x.Product).ThenInclude(x => x.ProductImages).Where(x => x.AppUserId == _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList();
                var bv = new BasketViewModel();
                foreach (var ci in basketItems)
                {
                    BasketItemViewModel bi = new BasketItemViewModel
                    {
                        Count = ci.Count,
                        Product = ci.Product
                    };
                    bv.BasketItems.Add(bi);
                    bv.TotalPrice += (bi.Product.DiscountPercent > 0 ? (bi.Product.SalePrice * (100 - bi.Product.DiscountPercent) / 100) : bi.Product.SalePrice) * bi.Count;
                }
                return bv;
            }
            else
            {
                var bv = new BasketViewModel();
                var basketJson = _httpContextAccessor.HttpContext.Request.Cookies["basket"];

                if (basketJson != null)
                {
                    var cookieItems = JsonConvert.DeserializeObject<List<BasketItemCookieViewModel>>(basketJson);

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
                }

                return bv;
            }
        }
    }
}
