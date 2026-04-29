using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Biblioteka.Web.Data;
using Biblioteka.Web.Models;
using Biblioteka.Web.Services;
using Biblioteka.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Biblioteka.Web.Helpers;
using LoginViewModel = Biblioteka.Web.Models.LoginViewModel;

namespace Biblioteka.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly BibliotekaDbContext _context;
        private readonly IEmailService _emailService;
        private readonly PasswordService _passwordService;

        public AccountController(BibliotekaDbContext context, IEmailService emailService, PasswordService passwordService)
        {
            _context = context;
            _emailService = emailService;
            _passwordService = passwordService;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var uzytkownik = await _context.Uzytkownicy
                .Include(u => u.Uprawnienia)
                .FirstOrDefaultAsync(u => u.Login == model.Login);

            if (uzytkownik == null)
            {
                ModelState.AddModelError("", "Niepoprawny login lub hasło.");
                return View(model);
            }

            if (uzytkownik.CzyZapomniany)
            {
                ModelState.AddModelError("", "Brak możliwości logowania - użytkownik zapomniany.");
                return View(model);
            }

            if (uzytkownik.CzyZablokowany && uzytkownik.BlokadaDo > DateTime.Now)
            {
                ModelState.AddModelError("", $"Przekroczono liczbę prób logowania. Konto zostało zablokowane.");
                return View(model);
            }

            if (uzytkownik.HasloHash == model.Password)
            {
                uzytkownik.LiczbaBlednychLogowan = 0;
                uzytkownik.CzyZablokowany = false;
                uzytkownik.BlokadaDo = null;
                await _context.SaveChangesAsync();

                if (uzytkownik.CzyHasloTymczasowe)
                {
                    return RedirectToAction("ForcePasswordChange", new { login = uzytkownik.Login });
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, uzytkownik.Login),
                    new Claim("FullName", $"{uzytkownik.Imie} {uzytkownik.Nazwisko}")
                };

                if (uzytkownik.Uprawnienia != null)
                {
                    foreach (var rola in uzytkownik.Uprawnienia)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, rola.Nazwa));
                    }
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties { IsPersistent = model.RememberMe });

                // --- POWRÓT DO TWOJEGO ORYGINALNEGO PRZEKIEROWANIA ---
                // Każda rola (w tym Manager) trafia tutaj:
                return RedirectToAction("Dashboard", "Uzytkownicy");
            }
            else
            {
                uzytkownik.LiczbaBlednychLogowan++;
                await _context.SaveChangesAsync();
                ModelState.AddModelError("", "Niepoprawny login lub hasło.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return Json(new { success = false });
            var user = await _context.Uzytkownicy.FirstOrDefaultAsync(u => u.Login == model.Login && u.Email == model.Email);
            if (user == null) return Json(new { success = false });
            string newPassword = _passwordService.GenerujHasloAutomatyczne();
            user.HasloHash = newPassword;
            user.CzyHasloTymczasowe = true;
            await _context.SaveChangesAsync();
            await _emailService.SendEmailAsync(user.Email, "Hasło", $"Hasło: {newPassword}");
            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult ForcePasswordChange(string login) => View(new WymuszenieZmianyHaslaViewModel { Login = login });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForcePasswordChange(WymuszenieZmianyHaslaViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _context.Uzytkownicy.FirstOrDefaultAsync(u => u.Login == model.Login);
            if (user == null) return NotFound();
            user.HasloHash = model.NoweHaslo!;
            user.CzyHasloTymczasowe = false;
            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }
    }
    
}