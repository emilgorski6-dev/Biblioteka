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


        [HttpGet]
        public IActionResult Dodaj()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Dodaj(DodajUzytkownikaViewModel model)
        {
            if (string.IsNullOrEmpty(model.Login) || string.IsNullOrEmpty(model.Email) ||
                string.IsNullOrEmpty(model.Imie) || string.IsNullOrEmpty(model.Nazwisko) ||
                string.IsNullOrEmpty(model.Pesel) || model.DataUrodzenia == default)
            {
                ModelState.AddModelError(string.Empty, "Nie uzupełniono wszystkich pól wymaganych");
            }
            if (model.DataUrodzenia > DateTime.Now)
                ModelState.AddModelError("DataUrodzenia", "Data urodzenia nie może być z przyszłości.");

            if (!string.IsNullOrEmpty(model.Telefon) && model.Telefon.Length != 9)
            {
                ModelState.AddModelError("Telefon", "Numer telefonu musi zawierać dokładnie 9 cyfr.");
            }

            if (!string.IsNullOrEmpty(model.Email))
            {
                if (model.Email.Count(c => c == '@') != 1)
                    ModelState.AddModelError("Email", "Nieprawidłowa liczba znaków @. Email musi zawierać dokładnie jeden znak @.");
                else if (model.Email.Length > 255)
                    ModelState.AddModelError("Email", "Niepoprawna długość adresu email. Adres email powinien zawierać maksymalnie 255 znaków.");
                else if (!EmailValidator.IsValid(model.Email))
                    ModelState.AddModelError("Email", "Błąd składni adresu email. Email powinien mieć format: nazwa_użytkownika@nazwa_domeny_serwera_poczty");
                else if (_context.Uzytkownicy.Any(u => u.Email == model.Email))
                    ModelState.AddModelError("Email", "Adres email został już zarejestrowany dla innego konta.");
            }

            if (!string.IsNullOrEmpty(model.Pesel))
            {
                var peselResult = PeselValidator.WalidujSzczegolowo(model.Pesel, model.DataUrodzenia!.Value, model.Plec, _context);

                if (!peselResult.IsValid)
                {
                    ModelState.AddModelError("Pesel", peselResult.ErrorMessage);
                }
            }

            if (_context.Uzytkownicy.Any(u => u.Login == model.Login))
                ModelState.AddModelError("Login", "Podany login jest już zajęty.");

            if (!ModelState.IsValid) return View(model);

            var user = new Uzytkownik
            {
                Login = model.Login,
                Imie = model.Imie,
                Nazwisko = model.Nazwisko,
                Pesel = model.Pesel,
                DataUrodzenia = model.DataUrodzenia!.Value,
                Plec = model.Plec,
                Email = model.Email,
                Telefon = model.Telefon,
                Miejscowosc = model.Miejscowosc,
                KodPocztowy = model.KodPocztowy,
                Ulica = model.Ulica,
                NumerPosesji = model.NumerPosesji,
                NumerLokalu = model.NumerLokalu
            };

            _context.Uzytkownicy.Add(user);
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Utworzono konto użytkownika ({user.Imie} {user.Nazwisko}).";
            return RedirectToAction("Index");
        }

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
                    Pesel = u.Pesel,
                    DataZapomnienia = u.DataZapomnienia
                })
                .ToList();

            ViewBag.CurrentLogin = searchLogin;
            ViewBag.CurrentName = searchName;
            ViewBag.CurrentPesel = searchPesel;

            return View(users);
        }

        public IActionResult Szczegoly(string login)
        {
            var user = _context.Uzytkownicy.FirstOrDefault(u => u.Login == login);
            if (user == null) return NotFound();

            return View(user);
        }
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

        [HttpPost]
        public IActionResult Edytuj(EdytujUzytkownikaViewModel model)
        {
            if (model.DataUrodzenia > DateTime.Now)
                ModelState.AddModelError("DataUrodzenia", "Data urodzenia nie może być z przyszłości.");

            if (!PhoneValidator.IsValid(model.Telefon))
                ModelState.AddModelError("Telefon", "Numer telefonu musi zawierać dokładnie 9 cyfr.");

            if (!string.IsNullOrEmpty(model.Email))
            {
                if (model.Email.Count(c => c == '@') != 1)
                    ModelState.AddModelError("Email", "Nieprawidłowa liczba znaków @. Email musi zawierać dokładnie jeden znak @.");
                else if (!EmailValidator.IsValid(model.Email))
                    ModelState.AddModelError("Email", "Błąd składni adresu email. Email powinien mieć format: nazwa_użytkownika@nazwa_domeny_serwera_poczty");
                else if (_context.Uzytkownicy.Any(u => u.Email == model.Email && u.Id != model.Id))
                    ModelState.AddModelError("Email", "Adres email został już zarejestrowany dla innego konta.");
            }
            string plecZFormatowana = string.IsNullOrEmpty(model.Plec) ? "" : char.ToUpper(model.Plec[0]) + model.Plec.Substring(1).ToLower();
            var wynikWalidacji = PeselValidator.WalidujSzczegolowo(model.Pesel, model.DataUrodzenia, plecZFormatowana, _context);
            if (!wynikWalidacji.IsValid) ModelState.AddModelError("Pesel", wynikWalidacji.ErrorMessage);

            if (_context.Uzytkownicy.Any(u => u.Login == model.Login && u.Id != model.Id))
                ModelState.AddModelError("Login", "Podany login jest już zajęty przez innego użytkownika.");

            if (_context.Uzytkownicy.Any(u => u.Email == model.Email && u.Id != model.Id))
                ModelState.AddModelError("Email", "Adres email został już zarejestrowany dla innego konta.");

            if (_context.Uzytkownicy.Any(u => u.Pesel == model.Pesel && u.Id != model.Id))
                ModelState.AddModelError("Pesel", "Podany nowy numer PESEL jest już przypisany do innego użytkownika w systemie.");

            if (!ModelState.IsValid) return View(model);

            var userToUpdate = _context.Uzytkownicy.Find(model.Id);
            if (userToUpdate == null) return NotFound();

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Zapomnij(int id)
        {
            var user = _context.Uzytkownicy
                            .Include(u => u.Uprawnienia)
                            .FirstOrDefault(u => u.Id == id);

            if (user == null) return NotFound();

            user.Uprawnienia.Clear();

            var anon = PeselValidator.GenerujDaneAnonimowe();

            user.Imie = Guid.NewGuid().ToString("N").Substring(0, 8);
            user.Nazwisko = Guid.NewGuid().ToString("N").Substring(0, 10);
            user.Pesel = anon.Pesel;
            user.DataUrodzenia = anon.DataUrodzenia;
            user.Plec = anon.Plec;
            user.ZapomnianyPrzezId = 1;
            
            user.CzyZapomniany = true;
            user.DataZapomnienia = DateTime.Now;

            
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Użytkownik został pomyślnie zapomniany, a jego uprawnienia usunięte.";
            return RedirectToAction("Zapomniani");
        }



    }
}