using Backend___Putka.DAL;
using Backend___Putka.Models;
using Backend___Putka.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Backend___Putka.Controllers
{
    public class OrderController : Controller
    {
        private readonly PutkaDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(PutkaDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Checkout()
        {
            OrderViewModel vm = new OrderViewModel();
            vm.CheckoutItems = GenerateCheckoutItems();

            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

                vm.Order = new OrderCreateViewModel
                {
                    Address = user.Address,
                    Email = user.Email,
                    UserName = user.UserName,
                    Phone = user.PhoneNumber
                };
            }

            vm.TotalPrice = vm.CheckoutItems.Any() ? vm.CheckoutItems.Sum(x => x.Price * x.Count) : 0;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateViewModel orderVM)
        {
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Member"))
            {
                if (string.IsNullOrWhiteSpace(orderVM.UserName))
                    ModelState.AddModelError("UserName", "UserName is required");

                if (string.IsNullOrWhiteSpace(orderVM.Email))
                    ModelState.AddModelError("Email", "Email is required");
            }

            if (!ModelState.IsValid)
            {
                OrderViewModel vm = new OrderViewModel();
                vm.CheckoutItems = GenerateCheckoutItems();
                vm.Order = orderVM;
                return View("Checkout", vm);
            }

            Order order = new Order
            {
                Address = orderVM.Address,
                Phone = orderVM.Phone,
                Note = orderVM.Note,
                Status = Enums.OrderStatus.Pending,
                CreatedDate = DateTime.UtcNow.AddHours(4)
            };

            var items = GenerateCheckoutItems();
            foreach (var item in items)
            {
                Product product = _context.Products.Find(item.ProductId);

                OrderItem orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Count = item.Count,
                };

                order.OrderItems.Add(orderItem);
            }

            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

                order.UserName = user.UserName;
                order.Email = user.Email;
                order.AppUserId = user.Id;

                ClearDbBasket(user.Id);
            }
            else
            {
                order.UserName = orderVM.UserName;
                order.Email = orderVM.Email;

                Response.Cookies.Delete("Basket");
            }

            _context.Orders.Add(order);
            _context.SaveChanges();

            return RedirectToAction("Order", "OrderSuccess");
        }

        private void ClearDbBasket(string userId)
        {
            _context.BasketItems.RemoveRange(_context.BasketItems.Where(x => x.AppUserId == userId).ToList());
            _context.SaveChanges();
        }

        private List<CheckoutItem> GenerateCheckoutItemsFromDb(string userId)
        {
            return _context.BasketItems.Include(x => x.Product).ThenInclude(x=>x.ProductImages).Where(x => x.AppUser.Id == userId).Select(x => new CheckoutItem
            {
                Count = x.Count,
                Name = x.Product.Name,
                ProductImages = x.Product.ProductImages,
                ProductId = x.ProductId,
                Price = x.Product.DiscountPercent > 0 ? (x.Product.SalePrice * (100 - x.Product.DiscountPercent) / 100) : x.Product.SalePrice
            }).ToList();
        }

        private List<CheckoutItem> GenerateCheckoutItemsFromCookie()
        {
            List<CheckoutItem> checkoutItems = new List<CheckoutItem>();
            var basketStr = Request.Cookies["basket"];

            if (basketStr != null)
            {
                List<BasketItemCookieViewModel> cookieItems = JsonConvert.DeserializeObject<List<BasketItemCookieViewModel>>(basketStr);

                foreach (var item in cookieItems)
                {
                    Product product = _context.Products.FirstOrDefault(x => x.Id == item.ProductId);

                    CheckoutItem checkoutItem = new CheckoutItem
                    {
                        Count = item.Count,
                        Name = product.Name,
                        ProductId = product.Id,
                        Price = product.DiscountPercent > 0 ? (product.SalePrice * (100 - product.DiscountPercent) / 100) : product.SalePrice
                    };
                    checkoutItems.Add(checkoutItem);
                }
            }

            return checkoutItems;
        }

        public List<CheckoutItem> GenerateCheckoutItems()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Member"))
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return GenerateCheckoutItemsFromDb(userId);
            }
            else
                return GenerateCheckoutItemsFromCookie();
        }


        public IActionResult OrderSuccess()
        {
            return View();
        }
    }
}
