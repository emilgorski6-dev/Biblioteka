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

        
        // --- ZU-01: Dodawanie użytkownika (GET) ---
        [HttpGet]
        public IActionResult Dodaj()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }
        // --- ZU-01: Dodawanie użytkownika (POST) ---
        [HttpPost]
        public IActionResult Dodaj(DodajUzytkownikaViewModel model)
        {
            // 1. Walidacja Pól Wymaganych (Punkt 5 z Use Case)
            if (string.IsNullOrEmpty(model.Login) || string.IsNullOrEmpty(model.Email) || 
                string.IsNullOrEmpty(model.Imie) || string.IsNullOrEmpty(model.Nazwisko) || 
                string.IsNullOrEmpty(model.Pesel) || model.DataUrodzenia == default)
            {
                ModelState.AddModelError(string.Empty, "Nie uzupełniono wszystkich pól wymaganych");
            }

            // 2. Walidacja Telefonu
            if (!string.IsNullOrEmpty(model.Telefon) && model.Telefon.Length != 9)
            {
                ModelState.AddModelError("Telefon", "Numer telefonu musi zawierać dokładnie 9 cyfr.");
            }

            // 3. Walidacja E-mail (Dokładne teksty z Use Case)
            if (!string.IsNullOrEmpty(model.Email))
                {
                    if (model.Email.Count(c => c == '@') != 1)
                        ModelState.AddModelError("Email", "Nieprawidłowa liczba znaków @. Email musi zawierać dokładnie jeden znak @.");
                    else if (model.Email.Length > 255)
                        ModelState.AddModelError("Email", "Niepoprawna długość adresu email. Adres email powinien zawierać maksymalnie 255 znaków.");
                    else if (!System.Text.RegularExpressions.Regex.IsMatch(model.Email, @"^[^@]+@[^@]+\.[^@]+$"))
                        ModelState.AddModelError("Email", "Błąd składni adresu email. Email powinien mieć format: nazwa_użytkownika@nazwa_domeny_serwera_poczty");
                    else if (_context.Uzytkownicy.Any(u => u.Email == model.Email))
                        ModelState.AddModelError("Email", "Adres email został już zarejestrowany dla innego konta.");
                }

            // 4. Walidacja PESEL (Używamy Twojej nowej metody WalidujSzczegolowo)
            if (!string.IsNullOrEmpty(model.Pesel))
            {
                // Ważne: Twoja metoda w helperze sprawdza "Mężczyzna"/"Kobieta" z dużej litery, 
                // a w select masz małe litery. Dodajemy .ToLower() w helperze lub tutaj poprawiamy:
                var (isValid, errorMessage) = PeselValidator.WalidujSzczegolowo(model.Pesel, model.DataUrodzenia, model.Plec);
                
                if (!isValid)
                {
                    ModelState.AddModelError("Pesel", errorMessage);
                }
            }

            // 5. Walidacja Unikalności (Baza danych) - Teksty z Use Case
            if (_context.Uzytkownicy.Any(u => u.Login == model.Login))
                ModelState.AddModelError("Login", "Podany login jest już zajęty.");

            if (_context.Uzytkownicy.Any(u => u.Email == model.Email))
                ModelState.AddModelError("Email", "Adres email został już zarejestrowany dla innego konta.");

            if (_context.Uzytkownicy.Any(u => u.Pesel == model.Pesel))
                ModelState.AddModelError("Pesel", "Podany numer PESEL jest już przypisany do innego użytkownika w systemie.");

            if (!ModelState.IsValid) return View(model);

            // Zapis do bazy...
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
                NumerLokalu = model.NumerLokalu
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

            if (!string.IsNullOrEmpty(model.Email))
                {
                    if (model.Email.Count(c => c == '@') != 1)
                        ModelState.AddModelError("Email", "Nieprawidłowa liczba znaków @. Email musi zawierać dokładnie jeden znak @.");
                    else if (!System.Text.RegularExpressions.Regex.IsMatch(model.Email, @"^[^@]+@[^@]+\.[^@]+$"))
                        ModelState.AddModelError("Email", "Błąd składni adresu email. Email powinien mieć format: nazwa_użytkownika@nazwa_domeny_serwera_poczty");
                    else if (_context.Uzytkownicy.Any(u => u.Email == model.Email && u.Id != model.Id))
                        ModelState.AddModelError("Email", "Adres email został już zarejestrowany dla innego konta.");
                }
            // Walidacja PESEL z formatowaniem płci
            string plecZFormatowana = string.IsNullOrEmpty(model.Plec) ? "" : char.ToUpper(model.Plec[0]) + model.Plec.Substring(1).ToLower();
            var wynikWalidacji = PeselValidator.WalidujSzczegolowo(model.Pesel, model.DataUrodzenia, plecZFormatowana);
            if (!wynikWalidacji.IsValid) ModelState.AddModelError("Pesel", wynikWalidacji.ErrorMessage);

            // Unikalność (z wyłączeniem siebie samego)
            if (_context.Uzytkownicy.Any(u => u.Login == model.Login && u.Id != model.Id))
                ModelState.AddModelError("Login", "Podany login jest już zajęty przez innego użytkownika.");

            if (_context.Uzytkownicy.Any(u => u.Email == model.Email && u.Id != model.Id))
                ModelState.AddModelError("Email", "Adres email został już zarejestrowany dla innego konta.");

            if (_context.Uzytkownicy.Any(u => u.Pesel == model.Pesel && u.Id != model.Id))
                ModelState.AddModelError("Pesel", "Podany nowy numer PESEL jest już przypisany do innego użytkownika w systemie.");

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

        // --- ZU-07: Zapomnienie użytkownika(POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Zapomnij(int id)
        {
            // 1. Pobieramy użytkownika razem z jego uprawnieniami
            var user = _context.Uzytkownicy
                            .Include(u => u.Uprawnienia)
                            .FirstOrDefault(u => u.Id == id);

            if (user == null) return NotFound();

            // 2. USUNIĘCIE UPRAWNIEŃ (Punkt 6 Use Case)
            // Dzięki .Clear() EF sam usunie rekordy z tabeli łączącej Uzytkownik_Uprawnienia
            user.Uprawnienia.Clear();

            // 3. GENEROWANIE DANYCH ANONIMOWYCH
            var anon = PeselValidator.GenerujDaneAnonimowe();

            // 4. NADPISYWANIE DANYCH (Anonimizacja RODO)
            user.Imie = "Anonim";
            user.Nazwisko = "Użytkownik";
            user.Pesel = anon.Pesel;
            user.DataUrodzenia = anon.DataUrodzenia;
            user.Plec = anon.Plec;

            // Czyszczenie kontaktu i adresu (zgodnie z Twoimi polami w klasie Uzytkownik)
            user.Email = $"zapomniany_{user.Id}@biblioteka.nova";
            user.Telefon = "000000000";
            user.Miejscowosc = "Anonimowa";
            user.KodPocztowy = "00-000";
            user.Ulica = null;
            user.NumerPosesji = "0";
            user.NumerLokalu = null;
            user.HasloHash = "---"; // Nadpisujemy hash hasła

            // 5. FLAGI SYSTEMOWE
            user.CzyZapomniany = true;
            user.DataZapomnienia = DateTime.Now;

            // NIE robimy CzyZablokowany = true (zgodnie z prośbą)

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Użytkownik został pomyślnie zapomniany, a jego uprawnienia usunięte.";
            return RedirectToAction("Zapomniani");
        }

        
    
    }
}