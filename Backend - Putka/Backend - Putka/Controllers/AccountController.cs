using Backend___Putka.DAL;
using Backend___Putka.Models;
using Backend___Putka.Services;
using Backend___Putka.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

            string emailContent = @"<!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />

    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />

    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />

    <link rel=""icon"" href=""images/favicon.png"" type=""image/x-icon"" />

    <title>Voxo | Email template</title>

    <link rel=""preconnect"" href=""https://fonts.googleapis.com"" />
    <link
      href=""https://fonts.googleapis.com/css2?family=Public+Sans:wght@100;200;300;400;500;600;700;800;900&display=swap""
      rel=""stylesheet""
    />
    <link
      href=""https://fonts.googleapis.com/css2?family=Nunito+Sans:wght@200;300;400;600;700;800;900&display=swap""
      rel=""stylesheet""
    />

    <style type=""text/css"">
      body {
        text-align: center;
        margin: 0 auto;
        width: 650px;
        font-family: ""Public Sans"", sans-serif;
        background-color: #e2e2e2;
        display: block;
      }

      ul {
        margin: 0;
        padding: 0;
      }

      li {
        display: inline-block;
        text-decoration: unset;
      }

      a {
        text-decoration: none;
      }

      h5 {
        margin: 10px;
        color: #777;
      }

      .text-center {
        text-align: center;
      }

      .header-menu ul li + li {
        margin-left: 20px;
      }

      .header-menu ul li a {
        font-size: 14px;
        color: #252525;
        font-weight: 500;
      }

      .password-button {
        background-color: #0da487;
        border: none;
        color: #fff;
        padding: 14px 26px;
        font-size: 18px;
        border-radius: 6px;
        font-weight: 600;
      }

      .footer-table {
        position: relative;
      }

      .footer-table::before {
        position: absolute;
        content: """";
        background-image: url(images/footer-left.svg);
        background-position: top right;
        top: 0;
        left: -71%;
        width: 100%;
        height: 100%;
        background-repeat: no-repeat;
        z-index: -1;
        background-size: contain;
        opacity: 0.3;
      }

      .footer-table::after {
        position: absolute;
        content: """";
        background-image: url(images/footer-right.svg);
        background-position: top right;
        top: 0;
        right: 0;
        width: 100%;
        height: 100%;
        background-repeat: no-repeat;
        z-index: -1;
        background-size: contain;
        opacity: 0.3;
      }

      .theme-color {
        color: #0da487;
      }
    </style>
  </head>

  <body style=""margin: 20px auto"">
    <table
      align=""center""
      border=""0""
      cellpadding=""0""
      cellspacing=""0""
      style=""
        background-color: white;
        width: 100%;
        box-shadow: 0px 0px 14px -4px rgba(0, 0, 0, 0.2705882353);
        -webkit-box-shadow: 0px 0px 14px -4px rgba(0, 0, 0, 0.2705882353);
      ""
    >
      <tbody>
        <tr>
          <td>
            <table
              class=""contant-table""
              style=""margin-top: 40px""
              align=""center""
              border=""0""
              cellpadding=""0""
              cellspacing=""0""
              width=""100%""
            >
              <thead>
                <tr style=""display: block"">
                  <td style=""display: block"">
                    <h3
                      style=""
                        font-weight: 700;
                        font-size: 20px;
                        margin: 0;
                        color: #939393;
                        margin-top: 15px;
                      ""
                    >
                      Hi Dear Customer,
                    </h3>
                  </td>

                  <td>
                    <p
                      style=""
                        font-size: 17px;
                        font-weight: 600;
                        width: 74%;
                        margin: 8px auto 0;
                        line-height: 1.5;
                        color: #939393;
                      ""
                    >
                      We’re Sending you this email because you requested a
                      password reset. click on this link to create a new
                      password:
                    </p>
                  </td>
                </tr>
              </thead>
            </table>

            <table
              class=""button-table""
              style=""margin-top: 27px; margin-left: 400px""
              align=""center""
              border=""0""
              cellpadding=""0""
              cellspacing=""0""
              width=""100%""
            >
              <thead>
                <tr style=""display: block"">
                  <td style=""display: block"">
                    <a href='" + url + @"' style='font-size: 16px; font-family: Arial, sans-serif; color: #ffffff; text-decoration: none; padding: 10px 20px; display: inline-block; border-radius: 5px; color: #ffffff;' class=""password-button"">Set a new password</a>
                  </td>
                </tr>
              </thead>
            </table>

            <table
              class=""contant-table""
              style=""margin-top: 27px""
              align=""center""
              border=""0""
              cellpadding=""0""
              cellspacing=""0""
              width=""100%""
            >
              <thead>
                <tr style=""display: block"">
                  <td style=""display: block"">
                    <p
                      style=""
                        font-size: 17px;
                        font-weight: 600;
                        width: 74%;
                        margin: 8px auto 0;
                        line-height: 1.5;
                        color: #939393;
                      ""
                    >
                      If you didn’t request a password reset, you can ignore
                      this email. your password will not be changed.
                    </p>
                  </td>
                </tr>
              </thead>
            </table>

            <table
              class=""text-center footer-table""
              align=""center""
              border=""0""
              cellpadding=""0""
              cellspacing=""0""
              width=""100%""
              style=""
                background-color: #282834;
                color: white;
                padding: 24px;
                overflow: hidden;
                z-index: 0;
                margin-top: 30px;
              ""
            >
              <tr>
                <td>
                  <table
                    border=""0""
                    cellpadding=""0""
                    cellspacing=""0""
                    class=""footer-social-icon text-center""
                    align=""center""
                    style=""margin: 8px auto 11px""
                  >
                    <tr>
                      <td>
                        <h4
                          style=""font-size: 19px; font-weight: 700; margin: 0""
                        >
                          Shop For <span class=""theme-color"">Putka</span>
                        </h4>
                      </td>
                    </tr>
                  </table>
                </td>
              </tr>
            </table>
          </td>
        </tr>
      </tbody>
    </table>
  </body>
</html>";

            _emailSender.Send(passwordVM.Email, "Reset Password", emailContent);
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

        public IActionResult GoogleLogin()
        {
            string url = Url.Action("googleresponse", "account", Request.Scheme);

            var prop = _signInManager.ConfigureExternalAuthenticationProperties("Google", url);

            return new ChallengeResult("Google", prop);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var info = _signInManager?.GetExternalLoginInfoAsync().Result;

            if (info == null) return RedirectToAction("login");

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);

            AppUser user = _userManager.FindByEmailAsync(email).Result;

            if (user == null)
            {
                user = new AppUser { Email = email, UserName = email };
                var result = _userManager.CreateAsync(user).Result;

                if (!result.Succeeded) return RedirectToAction("login");

                await _userManager.AddToRoleAsync(user, "Member");
            }

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction("index", "home");
        }
    }
}
