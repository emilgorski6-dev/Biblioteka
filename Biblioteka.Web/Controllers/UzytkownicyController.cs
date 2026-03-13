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
        
        // Pobranie formularza edycji po LOGINIE z paska URL (np. ?login=admin)
        [HttpGet]
        public IActionResult Edytuj(string login)
        {
            if (string.IsNullOrEmpty(login)) return BadRequest();

            // Szukamy użytkownika po loginie
            var user = _context.Uzytkownicy.FirstOrDefault(u => u.Login == login);
            
            if (user == null)
            {
                return NotFound(); // <-- TO ZWRACA BŁĄD 404!
            }

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

        // Odbiór danych z formularza
        [HttpPost]
        public IActionResult Edytuj(EdytujUzytkownikaViewModel model)
        {
            if (model.DataUrodzenia > DateTime.Now)
                ModelState.AddModelError("DataUrodzenia", "Data urodzenia nie może być z przyszłości.");

            if (!PhoneValidator.IsValid(model.Telefon))
                ModelState.AddModelError("Telefon", "Numer telefonu musi zawierać dokładnie 9 cyfr.");

            if (!EmailValidator.IsValid(model.Email))
                ModelState.AddModelError("Email", "Nieprawidłowy format email.");

            // --- TUTAJ JEST NOWA WALIDACJA PESEL ---
            string plecZFormatowana = string.IsNullOrEmpty(model.Plec) ? "" : char.ToUpper(model.Plec[0]) + model.Plec.Substring(1).ToLower();
            var wynikWalidacji = PeselValidator.WalidujSzczegolowo(model.Pesel, model.DataUrodzenia, plecZFormatowana);

            if (!wynikWalidacji.IsValid)
            {
                ModelState.AddModelError("Pesel", wynikWalidacji.ErrorMessage);
            }
            // ---------------------------------------

            // Walidacja unikalności - wykluczamy aktualnie edytowanego użytkownika (po ID z ukrytego pola)
            if (_context.Uzytkownicy.Any(u => u.Login == model.Login && u.Id != model.Id))
                ModelState.AddModelError("Login", "Podany login jest już zajęty przez innego użytkownika.");

            if (_context.Uzytkownicy.Any(u => u.Email == model.Email && u.Id != model.Id))
                ModelState.AddModelError("Email", "Ten adres email jest już powiązany z innym kontem.");

            if (_context.Uzytkownicy.Any(u => u.Pesel == model.Pesel && u.Id != model.Id))
                ModelState.AddModelError("Pesel", "Ten numer PESEL znajduje się już w bazie dla innego konta.");

            if (!ModelState.IsValid) return View(model);

            // Pobieramy użytkownika do aktualizacji na podstawie ukrytego ID
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
        // Formularz dodawania nowego użytkownika (Wyświetlenie pustej strony)
        [HttpGet]
        public IActionResult Dodaj()
        {
            return View();
        }

        // Akcja odbierająca dane z formularza po kliknięciu "Utwórz konto"
        [HttpPost]
        public IActionResult Dodaj(DodajUzytkownikaViewModel model)
        {
            if (model.DataUrodzenia > DateTime.Now)
            {
                ModelState.AddModelError("DataUrodzenia", "Data urodzenia nie może być z przyszłości.");
            }
            // Jeśli atrybuty [Required] z modelu nie zostały spełnione, wróć do widoku
            if (!ModelState.IsValid) return View(model);

            // 1. Walidacja formatu Telefonu (9 cyfr)
            if (!PhoneValidator.IsValid(model.Telefon))
                ModelState.AddModelError("Telefon", "Numer telefonu musi zawierać dokładnie 9 cyfr.");

            // 2. Walidacja Email (Format + @ + długość)
            if (!EmailValidator.IsValid(model.Email))
                ModelState.AddModelError("Email", "Nieprawidłowy format email (max 255 znaków, jeden znak @).");

            // 3. Walidacja PESEL (Logika matematyczna + Data + Płeć)
            // POPRAWKA: Użyto model.DataUrodzenia.Value
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

            // Jeśli po naszych własnych walidacjach pojawiły się błędy, wyświetl formularz ponownie (Toast to wyłapie)
            if (!ModelState.IsValid) return View(model);

            // Jeśli wszystko jest OK, tworzymy użytkownika
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
            // Po sukcesie wracamy do listy klientów
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