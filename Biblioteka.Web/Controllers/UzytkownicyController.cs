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
            {
                // Normalizacja: obie strony do małych liter
                query = query.Where(u => u.Login.ToLower().Contains(searchLogin.ToLower()));
            }

            if (!string.IsNullOrEmpty(searchName))
            {
                // Sprawdzamy imię lub nazwisko (case-insensitive)
                var searchLower = searchName.ToLower();
                query = query.Where(u => u.Imie.ToLower().Contains(searchLower)
                                      || u.Nazwisko.ToLower().Contains(searchLower));
            }
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

            if (!users.Any())
            {
                ViewData["Message"] = "Brak użytkowników serwisu";
            }

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
            // KROK 1: Czy pola wymagane są uzupełnione? (Podejście z nagrania)
            if (!ModelState.IsValid) return View(model);

            // KROK 2: Logika merytoryczna (Data, Telefon, Email, PESEL)
            var birthRes = BirthDateValidator.WalidujDateUrodzenia(model.DataUrodzenia!.Value);
            if (!birthRes.IsValid) ModelState.AddModelError("DataUrodzenia", birthRes.Message);

            // Przekazujemy _context do sprawdzenia unikalności numeru
            var phoneRes = PhoneValidator.WalidujNrTelefonu(model.Telefon ?? "", _context);
            if (!phoneRes.IsValid) ModelState.AddModelError("Telefon", phoneRes.Message);

            var emailRes = EmailValidator.WalidujEmail(model.Email, _context);
            if (!emailRes.IsValid) ModelState.AddModelError("Email", emailRes.ErrorMessage);

            var peselAlgRes = PeselValidator.WalidujAlgorytm(model.Pesel, model.DataUrodzenia, model.Plec!.Value);
            if (!peselAlgRes.IsValid) ModelState.AddModelError("Pesel", peselAlgRes.ErrorMessage);

            if (_context.Uzytkownicy.Any(u => u.Pesel == model.Pesel))
                ModelState.AddModelError("Pesel", "Ten numer PESEL jest już w bazie.");

            var loginResult = LoginValidator.WalidujLogin(model.Login, _context);
            if (!loginResult.IsValid) ModelState.AddModelError("Login", loginResult.Message);

            if (!ModelState.IsValid) return View(model);

            var user = new Uzytkownik
            {
                Login = model.Login,
                Imie = model.Imie,
                Nazwisko = model.Nazwisko,
                Pesel = model.Pesel,
                DataUrodzenia = model.DataUrodzenia!.Value,
                Plec = model.Plec.Value,
                Email = model.Email,
                Telefon = model.Telefon ?? "",
                Miejscowosc = model.Miejscowosc,
                KodPocztowy = model.KodPocztowy,
                NumerPosesji = model.NumerPosesji,
                Ulica = model.Ulica,
                NumerLokalu = model.NumerLokalu
            };

            var rolaKlient = _context.Uprawnienia.FirstOrDefault(r => r.Nazwa == "Klient");
            if (rolaKlient != null)
            {
                user.Uprawnienia = new List<Uprawnienie> { rolaKlient };
            }

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
            var user = _context.Uzytkownicy
                .Include(u => u.Uprawnienia) // Ładujemy relację uprawnień
                .FirstOrDefault(u => u.Login == login);

            if (user == null) return NotFound();

            // Przekazujemy listę nazw ról do widoku, aby zaznaczyć checkboxy w modalu
            ViewBag.ObecneRole = user.Uprawnienia.Select(r => r.Nazwa).ToList();

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
        [ValidateAntiForgeryToken]
        public IActionResult Edytuj(EdytujUzytkownikaViewModel model)
        {
            // KROK 1: Walidacja wstępna (puste pola)
            if (!ModelState.IsValid) return View(model);

            var userToUpdate = _context.Uzytkownicy.Find(model.Id);
            if (userToUpdate == null) return NotFound();

            // KROK 2: Sprawdzenie czy cokolwiek zmieniono (wymóg prowadzącego)
            bool bezZmian =
                userToUpdate.Imie == model.Imie &&
                userToUpdate.Nazwisko == model.Nazwisko &&
                userToUpdate.Pesel == model.Pesel &&
                userToUpdate.Email == model.Email &&
                userToUpdate.Telefon == model.Telefon &&
                userToUpdate.Plec == model.Plec &&
                userToUpdate.Miejscowosc == model.Miejscowosc &&
                userToUpdate.KodPocztowy == model.KodPocztowy &&
                userToUpdate.NumerPosesji == model.NumerPosesji &&
                userToUpdate.Ulica == model.Ulica &&
                userToUpdate.NumerLokalu == model.NumerLokalu;

            if (bezZmian)
            {
                ModelState.AddModelError(string.Empty, "Nie wprowadzono żadnych zmian do zapisania.");
                return View(model);
            }

            // KROK 3: Walidacje merytoryczne (wykonywane tylko gdy zaszły zmiany)

            // Walidacja Daty
            var birthRes = BirthDateValidator.WalidujDateUrodzenia(model.DataUrodzenia!.Value);
            if (!birthRes.IsValid) ModelState.AddModelError("DataUrodzenia", birthRes.Message);

            // Walidacja Email (z ID użytkownika, aby pominąć unikalność własnego maila)
            var emailRes = EmailValidator.WalidujEmail(model.Email, _context, model.Id);
            if (!emailRes.IsValid) ModelState.AddModelError("Email", emailRes.ErrorMessage);

            // Walidacja PESEL
            if (model.Plec.HasValue)
            {
                var peselAlgRes = PeselValidator.WalidujAlgorytm(model.Pesel, model.DataUrodzenia, model.Plec!.Value);
                if (!peselAlgRes.IsValid) ModelState.AddModelError("Pesel", peselAlgRes.ErrorMessage);
            }
            if (_context.Uzytkownicy.Any(u => u.Pesel == model.Pesel && u.Id != model.Id))
                ModelState.AddModelError("Pesel", "Ten numer PESEL jest już przypisany do innego użytkownika.");
            // POPRAWKA BŁĘDU: Dodano _context i model.Id do walidacji telefonu
            // model.Telefon ?? "" oznacza: weź numer telefonu, a jeśli jest nullem, użyj pustego tekstu.
            var phoneRes = PhoneValidator.WalidujNrTelefonu(model.Telefon ?? "", _context, model.Id);
            if (!phoneRes.IsValid) ModelState.AddModelError("Telefon", phoneRes.Message);

            if (!ModelState.IsValid) return View(model);

            // KROK 4: Aktualizacja danych (Loginu nie zmieniamy!)
            userToUpdate.Imie = model.Imie;
            userToUpdate.Nazwisko = model.Nazwisko;
            userToUpdate.Pesel = model.Pesel;
            userToUpdate.DataUrodzenia = model.DataUrodzenia!.Value;
            userToUpdate.Plec = model.Plec!.Value;
            userToUpdate.Email = model.Email;
            userToUpdate.Telefon = model.Telefon ?? "";
            userToUpdate.Miejscowosc = model.Miejscowosc;
            userToUpdate.KodPocztowy = model.KodPocztowy;
            userToUpdate.NumerPosesji = model.NumerPosesji;
            userToUpdate.Ulica = model.Ulica;
            userToUpdate.NumerLokalu = model.NumerLokalu;

            _context.SaveChanges();
            TempData["SuccessMessage"] = $"Zaktualizowano dane użytkownika ({userToUpdate.Imie} {userToUpdate.Nazwisko}).";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Zapomnij(int id)
        {
            var user = _context.Uzytkownicy.Find(id);
            if (user == null) return NotFound();

            var anon = PeselValidator.GenerujDaneAnonimowe();

            // Anonimizacja wszystkiego, co unikalne
            user.Imie = Guid.NewGuid().ToString("N").Substring(0, 5);
            user.Nazwisko = "RODO";
            user.Pesel = anon.Pesel;
            user.DataUrodzenia = anon.DataUrodzenia;
            user.Plec = anon.Plec;

            // KLUCZOWE: Czyścimy kontakt, aby zwolnić te dane w systemie
            user.Email = $"anon_{user.Id}@biblioteka.pl";
            user.Telefon = "000000000";

            user.CzyZapomniany = true;
            user.DataZapomnienia = DateTime.Now;
            user.ZapomnianyPrzezId = 1; // Tu ID zalogowanego admina

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Użytkownik został zapomniany i przeniesiony do archiwum.";
            return RedirectToAction("Zapomniani");
        }

        // Ekran uprawnień
        public IActionResult Uprawnienia()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ZapiszUprawnienia([FromBody] ZapiszUprawnieniaModel model)
        {
            // Krok 10 i Scenariusz wyjątku: Sprawdzenie czy zaznaczono przynajmniej jedno uprawnienie
            if (model == null || model.WybraneRole == null || !model.WybraneRole.Any())
            {
                return BadRequest(new { message = "Użytkownik musi posiadać co najmniej jedno uprawnienie" });
            }

            var user = _context.Uzytkownicy
                .Include(u => u.Uprawnienia)
                .FirstOrDefault(u => u.Login == model.Login);

            if (user == null) return NotFound();

            // Krok 11: Zapisanie w bazie danych
            var noweRole = _context.Uprawnienia
                .Where(r => model.WybraneRole.Contains(r.Nazwa))
                .ToList();

            user.Uprawnienia.Clear();
            foreach (var rola in noweRole)
            {
                user.Uprawnienia.Add(rola);
            }

            _context.SaveChanges();

            // Krok 12: Przygotowanie komunikatu z imieniem i nazwiskiem
            return Ok(new
            {
                message = $"Zmieniono uprawnienia użytkownikowi ({user.Imie} {user.Nazwisko})"
            });
        }
        // GET: /Uzytkownicy/ZmienHaslo?id=5
        [HttpGet]
        public IActionResult ZmienHaslo(int id)
        {
            var user = _context.Uzytkownicy.Find(id);
            if (user == null) return NotFound();

            var model = new ZmienHasloViewModel
            {
                Id = user.Id,
                Login = user.Login,
                PelnaNazwa = $"{user.Imie} {user.Nazwisko}"
            };
            return View(model);
        }

        // POST: /Uzytkownicy/ZmienHaslo
        [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult ZmienHaslo(ZmienHasloViewModel model)
            {
                if (!ModelState.IsValid) return View(model);

                var user = _context.Uzytkownicy.Find(model.Id);
                if (user == null) return NotFound();

                // Przekazujemy 'user', aby walidator sprawdził obecne hasło (user.HasloHash)
                var validationResult = PasswordValidator.Waliduj(model.NoweHaslo!, user, _context);
                
                if (!validationResult.IsValid)
                {
                    ModelState.AddModelError("NoweHaslo", validationResult.Message);
                    return View(model);
                }

                // 1. Archiwizacja starego hasła do historii (tylko jeśli nie było nullem)
                if (!string.IsNullOrEmpty(user.HasloHash))
                {
                    _context.HistoriaHasel.Add(new HistoriaHasla
                    {
                        UzytkownikId = user.Id,
                        HasloHash = user.HasloHash, // zapisujemy tekst jawny
                        DataNadania = DateTime.Now,
                        Uzytkownik = user,
                        CzyTymczasowe = false
                    });
                }

                // 2. Zapis nowego hasła (tekst jawny)
                user.HasloHash = model.NoweHaslo; 
                
                _context.SaveChanges();

                TempData["SuccessMessage"] = $"Zmieniono hasło użytkownikowi {user.Imie} {user.Nazwisko}";
                return RedirectToAction("Szczegoly", new { login = user.Login });
            }
    }

}