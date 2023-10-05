using Backend___Putka.DAL;
using Backend___Putka.Models;
using Backend___Putka.Services;
using Backend___Putka.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend___Putka.Controllers
{
    public class AccountController : Controller
    {
        private readonly PutkaDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(PutkaDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Profile()
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("login");
            }

            AccountProfileViewModel accountVM = new AccountProfileViewModel
            {
                Profile = new ProfileEditViewModel
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    Address = user.Address,
                    Phone = user.PhoneNumber,
                },

                Orders = _context.Orders.Include(x => x.OrderItems).ThenInclude(x => x.Product).ThenInclude(x => x.ProductImages).Where(x => x.AppUserId == user.Id).ToList(),

            };

            return View(accountVM);
        }

        [Authorize(Roles = "Member")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileEditViewModel profileVM)
        {
            if (!ModelState.IsValid)
            {
                AccountProfileViewModel vm = new AccountProfileViewModel { Profile = profileVM };
                return View(vm);
            }

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            user.Email = profileVM.Email;
            user.UserName = profileVM.UserName;
            user.Address = profileVM.Address;
            user.PhoneNumber = profileVM.Phone;

            if (!string.IsNullOrEmpty(profileVM.NewPassword))
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, profileVM.CurrentPassword, profileVM.NewPassword);

                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    AccountProfileViewModel vm = new AccountProfileViewModel { Profile = profileVM };
                    return View(vm);
                }
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                AccountProfileViewModel vm = new AccountProfileViewModel { Profile = profileVM };
                return View(vm);
            }

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction("profile");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(MemberRegisterViewModel memberVM)
        {
            if (!ModelState.IsValid) return View();

            if (_userManager.Users.Any(x => x.UserName == memberVM.UserName))
            {
                ModelState.AddModelError("UserName", "UserName is already taken");
                return View();
            }

            if (_userManager.Users.Any(x => x.Email == memberVM.Email))
            {
                ModelState.AddModelError("Email", "Email is already taken");
                return View();
            }

            AppUser user = new AppUser
            {
                UserName = memberVM.UserName,
                Email = memberVM.Email,
                IsAdmin = false
            };

            var result = await _userManager.CreateAsync(user, memberVM.Password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err.Description);
                return View();
            }

            await _userManager.AddToRoleAsync(user, "Member");

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction("index", "home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(MemberLoginViewModel loginVM, string returnUrl = null)
        {
            if (!ModelState.IsValid) return View();

            AppUser user = await _userManager.FindByNameAsync(loginVM.UserName);

            if (user == null || user.IsAdmin)
            {
                ModelState.AddModelError("", "UserName or Password incorrect");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "You don't have to permission to sign-in!");
                return View();
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "UserName or Password incorrect");
                return View();
            }

            return returnUrl != null ? Redirect(returnUrl) : RedirectToAction("index", "home");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel passwordVM)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = await _userManager.FindByEmailAsync(passwordVM.Email);

            if (user == null || user.IsAdmin)
            {
                ModelState.AddModelError("Email", "Email is not correct");
                return View();
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            string url = Url.Action("resetpassword", "account", new { email = passwordVM.Email, token = token }, Request.Scheme);
            url = url.Replace("\u0026", "&");

            _emailSender.Send(passwordVM.Email, "Reset Password", $"Click <a href=\"{url}\">here</a> to reset your password");
            return RedirectToAction("login");
        }

        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            AppUser user = await _userManager.FindByEmailAsync(email);

            if (user == null || user.IsAdmin || !await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token))
                return RedirectToAction("login");

            ViewBag.Email = email;
            ViewBag.Token = token;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetVM)
        {
            AppUser user = await _userManager.FindByEmailAsync(resetVM.Email);

            if (user == null || user.IsAdmin)
                return RedirectToAction("login");

            var result = await _userManager.ResetPasswordAsync(user, resetVM.Token, resetVM.Password);

            if (!result.Succeeded)
            {
                return RedirectToAction("login");
            }

            return RedirectToAction("login");
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("login", "account");
        }
    }
}
