using Microsoft.AspNetCore.Mvc;
using Biblioteka.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Biblioteka.Web.Controllers
{
    public class AccountController : Controller
    {
        // USUNIĘTO KONSTRUKTOR Z SIGNINMANAGER - nie potrzebujemy go!

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Wbudowane logowanie (tworzy sesję bez używania zewnętrznych pakietów)
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, model.Email) };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Dashboard", "Uzytkownicy");
        }

        [HttpGet]
        public IActionResult Register() => View();

        // --- AKCJA WYLOGOWANIA ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Wylogowuje z wbudowanych ciasteczek
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            // Przekierowanie do okna logowania
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult ChangePassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string NewPassword, string ConfirmPassword)
        {
            if (NewPassword != ConfirmPassword) return View();
            return RedirectToAction("Index", "Uzytkownicy");
        }
    }
}