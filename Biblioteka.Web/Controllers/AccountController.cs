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

            // 1. Sprawdzenie czy użytkownik w ogóle istnieje
            if (uzytkownik == null)
            {
                ModelState.AddModelError("", "Niepoprawny login lub hasło.");
                return View(model);
            }

            // 2. Obsługa RODO (użytkownik zapomniany)
            if (uzytkownik.CzyZapomniany)
            {
                ModelState.AddModelError("", "Brak możliwości logowania - użytkownik zapomniany.");
                return View(model);
            }

            // 3. Sprawdzenie aktywnej blokady czasowej (Wymaganie ze zdjęcia)
            if (uzytkownik.CzyZablokowany && uzytkownik.BlokadaDo.HasValue && uzytkownik.BlokadaDo > DateTime.Now)
            {
                ModelState.AddModelError("", $"Przekroczono liczbę prób logowania. Konto zostało zablokowane. Ponowne logowanie będzie możliwe o godzinie {uzytkownik.BlokadaDo.Value:HH:mm}");
                return View(model);
            }

            // 4. Weryfikacja hasła przy użyciu serwisu
            if (_passwordService.VerifyPassword(model.Password, uzytkownik.HasloHash ?? ""))
            {
                // Sukces - resetujemy liczniki błędów
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

                return RedirectToAction("Dashboard", "Uzytkownicy");
            }
            else
            {
                // Błędne hasło - zwiększamy licznik
                uzytkownik.LiczbaBlednychLogowan++;

                // SPRAWDZENIE LIMITU PRÓB (Wymaganie ze zdjęcia)
                if (uzytkownik.LiczbaBlednychLogowan >= 3)
                {
                    uzytkownik.CzyZablokowany = true;
                    // Blokujemy na 15 minut od teraz
                    uzytkownik.BlokadaDo = DateTime.Now.AddMinutes(20);
                    await _context.SaveChangesAsync();

                    ModelState.AddModelError("", $"Przekroczono liczbę prób logowania. Konto zostało zablokowane. Ponowne logowanie będzie możliwe o godzinie {uzytkownik.BlokadaDo.Value:HH:mm}");
                    return View(model);
                }

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

            var user = await _context.Uzytkownicy
                .Include(u => u.HistoriaHasel) // Ważne: dołączamy historię
                .FirstOrDefaultAsync(u => u.Login == model.Login && u.Email == model.Email);

            if (user == null)
            {
                return Json(new { success = false, message = "nie znaleziono użytkownika o podanych danych." });
            }

            // --- KLUCZOWA ZMIANA: Archiwizujemy stare, prawdziwe hasło zanim je skasujemy ---
            if (!string.IsNullOrEmpty(user.HasloHash))
            {
                _context.HistoriaHasel.Add(new HistoriaHasla
                {
                    UzytkownikId = user.Id,
                    Uzytkownik = user,
                    HasloHash = user.HasloHash,
                    DataNadania = DateTime.Now,
                    CzyTymczasowe = false // To było prawdziwe hasło użytkownika
                });
            }

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

            var user = await _context.Uzytkownicy
                .Include(u => u.Uprawnienia)
                .Include(u => u.HistoriaHasel)
                .FirstOrDefaultAsync(u => u.Login == model.Login);

            if (user == null) return NotFound();

            // Walidator teraz znajdzie stare hasło w tabeli historii!
            var validationResult = PasswordValidator.Waliduj(model.NoweHaslo!, user, _context);

            if (!validationResult.IsValid)
            {
                ModelState.AddModelError("", validationResult.Message);
                return View(model);
            }

            // Archiwizujemy hasło tymczasowe, żeby go nigdy więcej nie użył
            if (!string.IsNullOrEmpty(user.HasloHash))
            {
                _context.HistoriaHasel.Add(new HistoriaHasla
                {
                    UzytkownikId = user.Id,
                    Uzytkownik = user,
                    HasloHash = user.HasloHash,
                    DataNadania = DateTime.Now,
                    CzyTymczasowe = true // Oznaczamy jako tymczasowe (wymóg: blokada powrotu do tymczasowych)
                });
            }

            user.HasloHash = model.NoweHaslo!;
            user.CzyHasloTymczasowe = false;
            await _context.SaveChangesAsync();

            // Logowanie (bez zmian...)
            var claims = new List<Claim> {
        new Claim(ClaimTypes.Name, user.Login),
        new Claim("FullName", $"{user.Imie} {user.Nazwisko}")
    };
            if (user.Uprawnienia != null)
            {
                foreach (var rola in user.Uprawnienia) claims.Add(new Claim(ClaimTypes.Role, rola.Nazwa));
            }
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            TempData["SuccessMessage"] = "Zmieniono hasło do konta";
            return RedirectToAction("Index", "Home");
        }
    }

}