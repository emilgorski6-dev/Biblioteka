using Microsoft.AspNetCore.Mvc;
using Biblioteka.Web.Models;
using Biblioteka.Web.Helpers;
using Biblioteka.Web.Data;
using Biblioteka.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Biblioteka.Web.Controllers
{
    public class UzytkownicyController : Controller
    {
        private readonly BibliotekaDbContext _context;

        public UzytkownicyController(BibliotekaDbContext context)
        {
            _context = context;
        }

        // --- ZU-02 i ZU-03: Lista aktywnych klientów z funkcją wyszukiwania ---
        public IActionResult Index(string searchLogin, string searchName, string searchPesel)
        {
            var query = _context.Uzytkownicy
                .Where(u => u.CzyZapomniany == false)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchLogin))
                query = query.Where(u => u.Login.Contains(searchLogin));

            if (!string.IsNullOrEmpty(searchName))
                query = query.Where(u => u.Imie.Contains(searchName) || u.Nazwisko.Contains(searchName));

            if (!string.IsNullOrEmpty(searchPesel))
                query = query.Where(u => u.Pesel.Contains(searchPesel));

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

            ViewBag.CurrentLogin = searchLogin;
            ViewBag.CurrentName = searchName;
            ViewBag.CurrentPesel = searchPesel;

            return View(users);
        }

        // --- ZU-06: Edycja danych (GET) ---
        [HttpGet]
        public IActionResult Edytuj(string login)
        {
            if (string.IsNullOrEmpty(login)) return BadRequest();

            var user = _context.Uzytkownicy.FirstOrDefault(u => u.Login == login);
            if (user == null) return NotFound();

            var model = new EdytujUzytkownikaViewModel
            {
                Id = user.Id,
                Login = user.Login,
                Imie = user.Imie,
                Nazwisko = user.Nazwisko,
                Pesel = user.Pesel,
                DataUrodzenia = user.DataUrodzenia,
                Plec = user.Plec,
                Email = user.Email,
                Telefon = user.Telefon,
                Miejscowosc = user.Miejscowosc,
                KodPocztowy = user.KodPocztowy,
                Ulica = user.Ulica,
                NumerPosesji = user.NumerPosesji,
                NumerLokalu = user.NumerLokalu
            };

            return View(model);
        }

        // --- ZU-06: Edycja danych (POST) ---
        [HttpPost]
        public IActionResult Edytuj(EdytujUzytkownikaViewModel model)
        {
            if (model.DataUrodzenia > DateTime.Now)
                ModelState.AddModelError("DataUrodzenia", "Data urodzenia nie może być z przyszłości.");

            if (!PhoneValidator.IsValid(model.Telefon))
                ModelState.AddModelError("Telefon", "Numer telefonu musi zawierać dokładnie 9 cyfr.");

            if (!EmailValidator.IsValid(model.Email))
                ModelState.AddModelError("Email", "Nieprawidłowy format email.");

            // Walidacja PESEL z formatowaniem płci
            string plecZFormatowana = string.IsNullOrEmpty(model.Plec) ? "" : char.ToUpper(model.Plec[0]) + model.Plec.Substring(1).ToLower();
            var wynikWalidacji = PeselValidator.WalidujSzczegolowo(model.Pesel, model.DataUrodzenia, plecZFormatowana);
            if (!wynikWalidacji.IsValid) ModelState.AddModelError("Pesel", wynikWalidacji.ErrorMessage);

            // Unikalność (z wyłączeniem siebie samego)
            if (_context.Uzytkownicy.Any(u => u.Login == model.Login && u.Id != model.Id))
                ModelState.AddModelError("Login", "Podany login jest już zajęty przez innego użytkownika.");

            if (_context.Uzytkownicy.Any(u => u.Email == model.Email && u.Id != model.Id))
                ModelState.AddModelError("Email", "Ten adres email jest już powiązany z innym kontem.");

            if (_context.Uzytkownicy.Any(u => u.Pesel == model.Pesel && u.Id != model.Id))
                ModelState.AddModelError("Pesel", "Ten numer PESEL znajduje się już w bazie.");

            if (!ModelState.IsValid) return View(model);

            var userToUpdate = _context.Uzytkownicy.Find(model.Id);
            if (userToUpdate == null) return NotFound();

            // Przepisanie danych
            userToUpdate.Login = model.Login;
            userToUpdate.Imie = model.Imie;
            userToUpdate.Nazwisko = model.Nazwisko;
            userToUpdate.Pesel = model.Pesel;
            userToUpdate.DataUrodzenia = model.DataUrodzenia;
            userToUpdate.Plec = model.Plec;
            userToUpdate.Email = model.Email;
            userToUpdate.Telefon = model.Telefon;
            userToUpdate.Miejscowosc = model.Miejscowosc;
            userToUpdate.KodPocztowy = model.KodPocztowy;
            userToUpdate.Ulica = model.Ulica;
            userToUpdate.NumerPosesji = model.NumerPosesji;
            userToUpdate.NumerLokalu = model.NumerLokalu;

            _context.SaveChanges();
            TempData["SuccessMessage"] = $"Zaktualizowano dane użytkownika ({userToUpdate.Imie} {userToUpdate.Nazwisko}).";
            return RedirectToAction("Index");
        }

        // --- ZU-01: Dodawanie użytkownika (GET) ---
        [HttpGet]
        public IActionResult Dodaj()
        {
            return View();
        }

        // --- ZU-01: Dodawanie użytkownika (POST) ---
        [HttpPost]
        public IActionResult Dodaj(DodajUzytkownikaViewModel model)
        {
            if (model.DataUrodzenia > DateTime.Now)
                ModelState.AddModelError("DataUrodzenia", "Data urodzenia nie może być z przyszłości.");

            if (!ModelState.IsValid) return View(model);

            if (!PhoneValidator.IsValid(model.Telefon))
                ModelState.AddModelError("Telefon", "Numer telefonu musi zawierać dokładnie 9 cyfr.");

            if (!EmailValidator.IsValid(model.Email))
                ModelState.AddModelError("Email", "Nieprawidłowy format email.");

            if (!PeselValidator.CzyPeselJestPoprawny(model.Pesel, model.DataUrodzenia, model.Plec))
                ModelState.AddModelError("Pesel", "PESEL jest niepoprawny lub niezgodny z danymi.");

            if (_context.Uzytkownicy.Any(u => u.Login == model.Login))
                ModelState.AddModelError("Login", "Podany login jest już zajęty.");

            if (_context.Uzytkownicy.Any(u => u.Email == model.Email))
                ModelState.AddModelError("Email", "Ten adres email jest już zarejestrowany.");

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

            TempData["SuccessMessage"] = $"Utworzono konto użytkownika ({user.Imie} {user.Nazwisko}).";
            return RedirectToAction("Index");
        }

        // --- ZU-04: Lista zapomnianych użytkowników ---
        public IActionResult Zapomniani(string searchLogin, string searchName, string searchPesel)
        {
            var query = _context.Uzytkownicy
                .Where(u => u.CzyZapomniany == true)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchLogin))
                query = query.Where(u => u.Login.Contains(searchLogin));

            if (!string.IsNullOrEmpty(searchName))
                query = query.Where(u => u.Imie.Contains(searchName) || u.Nazwisko.Contains(searchName));

            if (!string.IsNullOrEmpty(searchPesel))
                query = query.Where(u => u.Pesel.Contains(searchPesel));

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

            ViewBag.CurrentLogin = searchLogin;
            ViewBag.CurrentName = searchName;
            ViewBag.CurrentPesel = searchPesel;

            return View(users);
        }

        // --- ZU-05: Szczegóły użytkownika ---
        public IActionResult Szczegoly(string login)
        {
            var user = _context.Uzytkownicy.FirstOrDefault(u => u.Login == login);
            if (user == null) return NotFound();

            return View(user);
        }
    }
}