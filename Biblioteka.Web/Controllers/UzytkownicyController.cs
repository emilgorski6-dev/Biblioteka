using Microsoft.AspNetCore.Mvc;
using Biblioteka.Web.Models;
using Biblioteka.Web.Helpers;
using Biblioteka.Web.Data;
using Biblioteka.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Biblioteka.Web.Controllers
{
    public class UzytkownicyController : Controller
    {
        private readonly BibliotekaDbContext _context;

        public UzytkownicyController(BibliotekaDbContext context)
        {
            _context = context;
        }

        // Widok główny Panelu (Dashboard)
        public IActionResult Dashboard()
        {
            return View();
        }

        // Lista aktywnych klientów z funkcją filtrowania
        public IActionResult Index(string searchLogin, string searchName, string searchPesel)
        {
            // 1. Pobieramy bazowe zapytanie (tylko aktywni)
            var query = _context.Uzytkownicy
                .Where(u => u.CzyZapomniany == false)
                .AsQueryable();

            // 2. Filtrowanie dynamiczne
            if (!string.IsNullOrEmpty(searchLogin))
                query = query.Where(u => u.Login.Contains(searchLogin));

            if (!string.IsNullOrEmpty(searchName))
                query = query.Where(u => u.Imie.Contains(searchName) || u.Nazwisko.Contains(searchName));

            if (!string.IsNullOrEmpty(searchPesel))
                query = query.Where(u => u.Pesel.Contains(searchPesel));

            // 3. Mapowanie na ViewModel (Pesel jest wymagany przez model)
            var users = query
                .Select(u => new UzytkownikListItemViewModel
                {
                    Login = u.Login,
                    Imie = u.Imie,
                    Nazwisko = u.Nazwisko,
                    Email = u.Email,
                    Pesel = u.Pesel 
                })
                .ToList();

            // 4. Przekazanie filtrów do widoku
            ViewBag.CurrentLogin = searchLogin;
            ViewBag.CurrentName = searchName;
            ViewBag.CurrentPesel = searchPesel;

            return View(users);
        }

        // Formularz dodawania nowego użytkownika (GET)
        [HttpGet]
        public IActionResult Dodaj()
        {
            return View();
        }

        // Akcja odbierająca dane z formularza (POST)
        [HttpPost]
        public IActionResult Dodaj(DodajUzytkownikaViewModel model)
        {
            if (model.DataUrodzenia > DateTime.Now)
            {
                ModelState.AddModelError("DataUrodzenia", "Data urodzenia nie może być z przyszłości.");
            }

            if (!ModelState.IsValid) return View(model);

            // Walidacje pomocnicze (Helpers)
            if (!PhoneValidator.IsValid(model.Telefon))
                ModelState.AddModelError("Telefon", "Numer telefonu musi zawierać dokładnie 9 cyfr.");

            if (!EmailValidator.IsValid(model.Email))
                ModelState.AddModelError("Email", "Nieprawidłowy format email.");

            if (!PeselValidator.CzyPeselJestPoprawny(model.Pesel, model.DataUrodzenia, model.Plec))
                ModelState.AddModelError("Pesel", "PESEL jest niepoprawny lub niezgodny z danymi.");

            // Unikalność w bazie danych
            if (_context.Uzytkownicy.Any(u => u.Login == model.Login))
                ModelState.AddModelError("Login", "Podany login jest już zajęty.");

            if (_context.Uzytkownicy.Any(u => u.Email == model.Email))
                ModelState.AddModelError("Email", "Ten adres email jest już zarejestrowany.");

            if (_context.Uzytkownicy.Any(u => u.Pesel == model.Pesel))
                ModelState.AddModelError("Pesel", "Ten numer PESEL znajduje się już w bazie.");

            if (!ModelState.IsValid) return View(model);

            // Tworzenie encji
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

            TempData["SuccessMessage"] = $"Utworzono konto użytkownika ({user.Imie} {user.Nazwisko}).";
            return RedirectToAction("Index");
        }

        // Lista zablokowanych użytkowników (Zapomniani)
        public IActionResult Zapomniani(string searchLogin, string searchName, string searchPesel)
        {
            // 1. Pobieramy tylko tych, którzy są oznaczeni jako zablokowani
            var query = _context.Uzytkownicy
                .Where(u => u.CzyZapomniany == true)
                .AsQueryable();

            // 2. Filtrowanie dynamiczne
            if (!string.IsNullOrEmpty(searchLogin))
                query = query.Where(u => u.Login.Contains(searchLogin));

            if (!string.IsNullOrEmpty(searchName))
                query = query.Where(u => u.Imie.Contains(searchName) || u.Nazwisko.Contains(searchName));

            if (!string.IsNullOrEmpty(searchPesel))
                query = query.Where(u => u.Pesel.Contains(searchPesel));

            // 3. Mapowanie na ViewModel
            var users = query
                .Select(u => new UzytkownikListItemViewModel
                {
                    Login = u.Login,
                    Imie = u.Imie,
                    Nazwisko = u.Nazwisko,
                    Email = u.Email,
                    Pesel = u.Pesel
                })
                .ToList();

            // 4. Przekazanie filtrów do widoku
            ViewBag.CurrentLogin = searchLogin;
            ViewBag.CurrentName = searchName;
            ViewBag.CurrentPesel = searchPesel;

            return View(users);
        }

        // Szczegóły konkretnego klienta
        public IActionResult Szczegoly(string login)
        {
            var user = _context.Uzytkownicy
                .FirstOrDefault(u => u.Login == login);

            if (user == null) return NotFound();

            return View(user);
        }
    }
}