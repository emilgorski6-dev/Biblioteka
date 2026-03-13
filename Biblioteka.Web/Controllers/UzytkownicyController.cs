using Microsoft.AspNetCore.Mvc;
using Biblioteka.Web.Models;
using Biblioteka.Web.Helpers;
using Biblioteka.Web.Data;
using Biblioteka.Web.Data.Entities;

namespace Biblioteka.Web.Controllers
{
    public class UzytkownicyController : Controller
    {
        private readonly BibliotekaDbContext _context;

        public UzytkownicyController(BibliotekaDbContext context)
        {
            _context = context;
        }

        // Widok główny Panelu (Ten z ikoną tarczy)
        public IActionResult Dashboard()
        {
            return View();
        }

        // Lista aktywnych klientów
        public IActionResult Index()
        {
            var users = _context.Uzytkownicy
                .Select(u => new UzytkownikListItemViewModel
                {
                    Login = u.Login,
                    Imie = u.Imie,
                    Nazwisko = u.Nazwisko,
                    Email = u.Email
                })
                .ToList();

            return View(users);
        }

        // Formularz dodawania nowego użytkownika
        public IActionResult Dodaj()
        {
            return View();
        }

        // Akcja odbierająca dane z formularza dodawania
        [HttpPost]
        public IActionResult Dodaj(DodajUzytkownikaViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // 1. Walidacja formatu Telefonu (9 cyfr)
            if (!PhoneValidator.IsValid(model.Telefon))
                ModelState.AddModelError("Telefon", "Numer telefonu musi zawierać dokładnie 9 cyfr.");

            // 2. Walidacja Email (Format + @ + długość)
            if (!EmailValidator.IsValid(model.Email))
                ModelState.AddModelError("Email", "Nieprawidłowy format email (max 255 znaków, jeden znak @).");

            // 3. Walidacja PESEL (Logika matematyczna + Data + Płeć)
            if (!PeselValidator.CzyPeselJestPoprawny(model.Pesel, model.DataUrodzenia, model.Plec))
                ModelState.AddModelError("Pesel", "PESEL jest niepoprawny lub niezgodny z datą urodzenia/płcią.");

            // --- KORELACJA Z BAZĄ DANYCH (UNIKALNOŚĆ) ---

            // 4. Unikalność Login
            if (_context.Uzytkownicy.Any(u => u.Login == model.Login))
                ModelState.AddModelError("Login", "Podany login jest już zajęty.");

            // 5. Unikalność Email
            if (_context.Uzytkownicy.Any(u => u.Email == model.Email))
                ModelState.AddModelError("Email", "Ten adres email jest już zarejestrowany.");

            // 6. Unikalność PESEL
            if (_context.Uzytkownicy.Any(u => u.Pesel == model.Pesel))
                ModelState.AddModelError("Pesel", "Ten numer PESEL znajduje się już w bazie.");

            if (!ModelState.IsValid) return View(model);

            var user = new Uzytkownik
            {
                Login = model.Login,
                Imie = model.Imie,
                Nazwisko = model.Nazwisko,
                Pesel = model.Pesel,
                DataUrodzenia = model.DataUrodzenia,
                Plec = model.Plec,
                Email = model.Email,
                Telefon = model.Telefon,
                Miejscowosc = model.Miejscowosc,
                KodPocztowy = model.KodPocztowy,
                Ulica = model.Ulica,
                NumerPosesji = model.NumerPosesji,
                NumerLokalu = model.NumerLokalu,
                CzyZablokowany = false,
                LiczbaBlednychLogowan = 0,
                CzyZapomniany = false
            };

            _context.Uzytkownicy.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Lista zablokowanych użytkowników
        public IActionResult Zapomniani()
        {
            return View();
        }

        // Szczegóły konkretnego klienta
        public IActionResult Szczegoly(string login)
        {
            var user = new
            {
                Login = login,
                Imie = "Jan",
                Nazwisko = "Kowalski",
                Email = "jan.kowalski@example.com",
                Telefon = "123456789"
            };

            return View(user);
        }
    }
}
