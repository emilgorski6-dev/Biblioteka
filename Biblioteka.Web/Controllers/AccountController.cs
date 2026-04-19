using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Biblioteka.Web.Data;
using Biblioteka.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteka.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly BibliotekaDbContext _context;

        public AccountController(BibliotekaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Scenariusz wyjątku: Brak wypełnienia pól (obsługiwane przez ModelState)
            if (!ModelState.IsValid) return View(model);

            var uzytkownik = await _context.Uzytkownicy
                .Include(u => u.Uprawnienia) // Pobieramy role z bazy danych
                .FirstOrDefaultAsync(u => u.Login == model.Login);

            // Scenariusz wyjątku: Niepoprawne dane logowania (login nie istnieje)
            if (uzytkownik == null)
            {
                ModelState.AddModelError("", "Niepoprawny login lub hasło.");
                return View(model);
            }

            // Scenariusz wyjątku: Próba zalogowania na konto zapomniane (RODO)
            if (uzytkownik.CzyZapomniany)
            {
                ModelState.AddModelError("", "Brak możliwości logowania - użytkownik zapomniany.");
                return View(model);
            }

            // Scenariusz wyjątku: Próba logowania na czasowo zablokowane konto (Wymaganie L-01)
            if (uzytkownik.CzyZablokowany && uzytkownik.BlokadaDo > DateTime.Now)
            {
                ModelState.AddModelError("", $"Przekroczono liczbę prób logowania. Konto zostało zablokowane. \nPonowne logowanie będzie możliwe o godzinie {uzytkownik.BlokadaDo.Value:HH:mm}");
                return View(model);
            }

            // Weryfikacja hasła
            bool passwordValid = uzytkownik.HasloHash == model.Password;

            if (passwordValid)
            {
                // Scenariusz główny L-01: Sukces - Resetujemy liczniki blokad
                uzytkownik.LiczbaBlednychLogowan = 0;
                uzytkownik.CzyZablokowany = false;
                uzytkownik.BlokadaDo = null;
                await _context.SaveChangesAsync();

                // Tworzenie tożsamości użytkownika (Claims)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, uzytkownik.Login),
                    new Claim("FullName", $"{uzytkownik.Imie} {uzytkownik.Nazwisko}")
                };

                // Dodanie ról do sesji (Wymaganie funkcjonalne nr 2 i 4)
                if (uzytkownik.Uprawnienia != null)
                {
                    foreach (var rola in uzytkownik.Uprawnienia)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, rola.Nazwa));
                    }
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Autentykacja - tworzenie ciasteczka sesji
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties { IsPersistent = model.RememberMe });

                // PRZEKIEROWANIE: Po zalogowaniu Admin/Użytkownik trafia do swojego panelu
                return RedirectToAction("Dashboard", "Uzytkownicy");
            }
            else
            {
                // Scenariusz wyjątku: Błędne hasło + Obsługa blokady czasowej (Wymaganie L-01)
                uzytkownik.LiczbaBlednychLogowan++;

                if (uzytkownik.LiczbaBlednychLogowan >= 3)
                {
                    uzytkownik.CzyZablokowany = true;
                    uzytkownik.BlokadaDo = DateTime.Now.AddMinutes(20); // Blokada na 20 minut

                    await _context.SaveChangesAsync();
                    ModelState.AddModelError("", $"Przekroczono liczbę prób logowania. Konto zostało zablokowane. \nPonowne logowanie będzie możliwe o godzinie {uzytkownik.BlokadaDo.Value:HH:mm}");
                }
                else
                {
                    await _context.SaveChangesAsync();
                    ModelState.AddModelError("", "Niepoprawny login lub hasło.");
                }

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Wylogowanie z systemu - usunięcie ciasteczka
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // PRZEKIEROWANIE: Po wylogowaniu wracamy na stronę główną
            return RedirectToAction("Index", "Home");
        }
    }
}