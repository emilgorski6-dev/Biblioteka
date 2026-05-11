using Microsoft.AspNetCore.Mvc;
using Biblioteka.Web.Models;
using Biblioteka.Web.Data;
using Biblioteka.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Biblioteka.Web.Controllers
{
    public class KsiazkiController : Controller
    {
        private readonly BibliotekaDbContext _context;

        public KsiazkiController(BibliotekaDbContext context)
        {
            _context = context;
        }

        // --- MOCK BAZY WYPOŻYCZEŃ ---
        public static List<WypozyczenieViewModel> _wypozyczenia = new List<WypozyczenieViewModel>
        {
            new WypozyczenieViewModel { Id = 1, Wypozyczajacy = "Jan Kowalski", Ksiazka = "Duma i uprzedzenie", DataWypozyczenia = new DateTime(2026, 4, 10), DataZwrotu = new DateTime(2026, 4, 24), Status = "Nowe", Bibliotekarz = "Anna Bibliotekarz" },
            new WypozyczenieViewModel { Id = 2, Wypozyczajacy = "Magdalena Lewandowska", Ksiazka = "Harry Potter i Kamień Filozoficzny", DataWypozyczenia = new DateTime(2026, 3, 15), DataZwrotu = new DateTime(2026, 5, 15), Status = "Przedłużone", Bibliotekarz = "Piotr Wiśniewski" },
            new WypozyczenieViewModel { Id = 3, Wypozyczajacy = "Piotr Wiśniewski", Ksiazka = "Władca Pierścieni: Drużyna Pierścienia", DataWypozyczenia = new DateTime(2026, 1, 10), DataZwrotu = new DateTime(2026, 1, 24), Status = "Zakończone", Bibliotekarz = "Anna Bibliotekarz" }
        };

        // --- ZRK-01: LISTA KSIĄŻEK (Dla Bibliotekarza/Użytkownika) ---
        [HttpGet]
        [Authorize(Roles = "Bibliotekarz,Manager")]
        public async Task<IActionResult> Lista(KsiazkaFiltrViewModel filtr)
        {
            var query = _context.Ksiazki.AsQueryable();

            // FILTR TYTUŁU: Teraz ignoruje wielkość liter
            if (!string.IsNullOrEmpty(filtr.Tytul))
            {
                query = query.Where(k => k.Tytul.ToLower().Contains(filtr.Tytul.ToLower()));
            }

            if (filtr.WybraniAutorzy.Any())
                query = query.Where(k => filtr.WybraniAutorzy.Contains(k.Autorzy));

            if (filtr.WybraneGatunki.Any())
                query = query.Where(k => filtr.WybraneGatunki.Contains(k.Gatunek));

            if (filtr.WybraneWydawnictwa.Any())
                query = query.Where(k => filtr.WybraneWydawnictwa.Contains(k.Wydawnictwo));

            if (filtr.WybraneStatusy.Any())
                query = query.Where(k => filtr.WybraneStatusy.Contains(k.Status));

            filtr.Wyniki = await query.Select(k => new KsiazkaListaViewModel
            {
                Id = k.Id,
                Tytul = k.Tytul,
                Autor = k.Autorzy,
                Gatunek = k.Gatunek,
                Wydawnictwo = k.Wydawnictwo,
                Status = k.Status,
                RokWydania = k.RokWydania
            }).ToListAsync();

            filtr.DostepniAutorzy = await _context.Ksiazki.Select(k => k.Autorzy).Distinct().OrderBy(a => a).ToListAsync();
            filtr.DostepneGatunki = await _context.Ksiazki.Select(k => k.Gatunek).Distinct().OrderBy(g => g).ToListAsync();
            filtr.DostepneWydawnictwa = await _context.Ksiazki.Select(k => k.Wydawnictwo).Distinct().OrderBy(w => w).ToListAsync();

            filtr.DostepneGatunki = new List<string>
            {
                "Fantastyka", "Kryminał", "Literatura piękna", "Nauka", 
                "Thriller", "Biografia", "Historyczna", "Horror"
            };

            return View(filtr);
        }

        // --- ZRK-02: LISTA REJESTRACJI (Dla Managera) ---
        [HttpGet]
        [Authorize(Roles = "Administrator,Manager")] 
        public async Task<IActionResult> ListaRejestracji(
            string[] autorzy, string[] gatunki, string tytul, string wydawnictwo, 
            DateTime? dataOd, DateTime? dataDo, string[] pracownicy)
        {
            if (dataOd.HasValue && dataDo.HasValue && dataOd > dataDo)
            {
                ModelState.AddModelError("DataRange", "Data początkowa nie może być późniejsza niż data końcowa.");
            }

            var query = _context.Ksiazki.AsQueryable();

            if (!string.IsNullOrEmpty(tytul))
                query = query.Where(k => k.Tytul.Contains(tytul));

            if (!string.IsNullOrEmpty(wydawnictwo))
                query = query.Where(k => k.Wydawnictwo.Contains(wydawnictwo));

            if (autorzy != null && autorzy.Length > 0)
                query = query.Where(k => autorzy.Contains(k.Autorzy));

            if (gatunki != null && gatunki.Length > 0)
                query = query.Where(k => gatunki.Contains(k.Gatunek));

            if (pracownicy != null && pracownicy.Length > 0)
                query = query.Where(k => pracownicy.Contains(k.OsobaRejestrujaca));

            if (dataOd.HasValue)
                query = query.Where(k => k.DataRejestracji >= dataOd.Value);

            if (dataDo.HasValue)
                query = query.Where(k => k.DataRejestracji <= dataDo.Value);

            ViewBag.IsFilterActive = (autorzy?.Length > 0 || gatunki?.Length > 0 || !string.IsNullOrEmpty(tytul) ||
                                     !string.IsNullOrEmpty(wydawnictwo) || dataOd.HasValue || dataDo.HasValue || pracownicy?.Length > 0);

            var model = await query.Select(k => new KsiazkaListaViewModel
            {
                Id = k.Id, Tytul = k.Tytul, Autor = k.Autorzy, Gatunek = k.Gatunek,
                Wydawnictwo = k.Wydawnictwo, DataRejestracji = k.DataRejestracji, OsobaRejestrujaca = k.OsobaRejestrujaca
            }).ToListAsync();

            ViewBag.DostepniAutorzy = await _context.Ksiazki.Select(k => k.Autorzy).Distinct().ToListAsync();
            ViewBag.DostepneGatunki = await _context.Ksiazki.Select(k => k.Gatunek).Distinct().ToListAsync();
            ViewBag.Pracownicy = await _context.Ksiazki.Select(k => k.OsobaRejestrujaca).Distinct().ToListAsync();

            return View(model);
        }

        [HttpGet]
        public IActionResult Zarejestruj()
        {
            PrepareGenreList();
            return View(new KsiazkaListaViewModel());
        }

        [HttpGet]
        public async Task<IActionResult> ZarejestrujZListy(int id)
        {
            var istniejaca = await _context.Ksiazki.FindAsync(id);
            if (istniejaca == null) return NotFound();

            var nowyEgzemplarz = new KsiazkaListaViewModel
            {
                Id = istniejaca.Id, Tytul = istniejaca.Tytul, Autor = istniejaca.Autorzy, Gatunek = istniejaca.Gatunek
            };

            PrepareGenreList();
            return View("Zarejestruj", nowyEgzemplarz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Zarejestruj(KsiazkaListaViewModel model)
        {
            if (model.RokWydania.HasValue && model.RokWydania > DateTime.Now.Year)
            {
                ModelState.AddModelError("RokWydania", "Rok wydania nie może być z przyszłości");
            }

            if (ModelState.IsValid)
            {
                var nowaKsiazka = new Ksiazka
                {
                    Tytul = model.Tytul, Autorzy = model.Autor, Gatunek = model.Gatunek,
                    Wydawnictwo = model.Wydawnictwo, RokWydania = model.RokWydania ?? DateTime.Now.Year,
                    LiczbaStron = model.LiczbaStron ?? 0, Cena = model.Cena ?? 0,
                    LiczbaSztuk = model.LiczbaSztuk ?? 0, Opis = model.Opis ?? "", Status = "Dostępna",
                    DataRejestracji = DateTime.Now, OsobaRejestrujaca = User.Identity?.Name ?? "System"
                };

                _context.Ksiazki.Add(nowaKsiazka);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = model.Tytul;
                return RedirectToAction(nameof(Lista));
            }

            PrepareGenreList();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Szczegoly(int id)
        {
            var ksiazka = await _context.Ksiazki.FirstOrDefaultAsync(k => k.Id == id);
            if (ksiazka == null) return NotFound();

            var model = new KsiazkaListaViewModel
            {
                Id = ksiazka.Id, Tytul = ksiazka.Tytul, Autor = ksiazka.Autorzy, Gatunek = ksiazka.Gatunek,
                Wydawnictwo = ksiazka.Wydawnictwo, RokWydania = ksiazka.RokWydania, LiczbaStron = ksiazka.LiczbaStron,
                Cena = ksiazka.Cena, LiczbaSztuk = ksiazka.LiczbaSztuk, Opis = ksiazka.Opis, Status = ksiazka.Status,
                DataRejestracji = ksiazka.DataRejestracji, OsobaRejestrujaca = ksiazka.OsobaRejestrujaca
            };

            return View(model);
        }

        private void PrepareGenreList()
        {
            ViewBag.Gatunki = new List<SelectListItem>
            {
                new() { Value = "Fantastyka", Text = "Fantastyka" }, new() { Value = "Kryminał", Text = "Kryminał" },
                new() { Value = "Literatura piękna", Text = "Literatura piękna" }, new() { Value = "Nauka", Text = "Nauka" },
                new() { Value = "Thriller", Text = "Thriller" }, new() { Value = "Biografia", Text = "Biografia" },
                new() { Value = "Historyczna", Text = "Historyczna" }, new() { Value = "Horror", Text = "Horror" }
            };
        }
        [HttpGet]
        public async Task<JsonResult> PobierzKlientow(string fraza)
        {
            if (string.IsNullOrWhiteSpace(fraza)) return Json(new List<object>());

            var slowa = fraza.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var query = _context.Uzytkownicy.AsQueryable();

            foreach (var slowo in slowa)
            {
                // Używamy EF.Functions.Like, który w SQLite lepiej radzi sobie z ignorowaniem wielkości liter
                // Dodajemy ?? "", żeby NULL w bazie nie zepsuł wyszukiwania
                string s = $"%{slowo}%";
                query = query.Where(u => 
                    EF.Functions.Like(u.Imie ?? "", s) || 
                    EF.Functions.Like(u.Nazwisko ?? "", s) || 
                    EF.Functions.Like(u.Miejscowosc ?? "", s) || 
                    EF.Functions.Like(u.Ulica ?? "", s) || 
                    EF.Functions.Like(u.Telefon ?? "", s) ||
                    EF.Functions.Like(u.NumerPosesji ?? "", s)
                );
            }

            var klienci = await query
                .Select(u => new { 
                    id = u.Id, 
                    nazwa = u.Imie + " " + u.Nazwisko, 
                    telefon = u.Telefon, 
                    adres = (u.Miejscowosc ?? "") + ", ul. " + (u.Ulica ?? "") + " " + (u.NumerPosesji ?? "")
                })
                .Take(10)
                .ToListAsync();

            return Json(klienci);
        }

        [HttpGet]
        public async Task<JsonResult> PobierzKsiazki(string fraza)
        {
            // 1. Sprawdzamy, czy w całej bibliotece jest jakakolwiek książka "Dostępna"
            // To obsługuje Twój case: "Brak książek ze statusem Dostępna"
            bool czySaJakiekolwiekDostepne = await _context.Ksiazki.AnyAsync(k => k.Status == "Dostępna");

            if (!czySaJakiekolwiekDostepne)
            {
                return Json(new { brakDostepnych = true });
            }

            if (string.IsNullOrWhiteSpace(fraza)) return Json(new List<object>());

            // 2. Rozbijamy frazę na słowa (kolejność nie ma znaczenia)
            var slowa = fraza.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var query = _context.Ksiazki.Where(k => k.Status == "Dostępna").AsQueryable();

            foreach (var slowo in slowa)
            {
                string s = $"%{slowo}%";
                query = query.Where(k => 
                    EF.Functions.Like(k.Tytul ?? "", s) || 
                    EF.Functions.Like(k.Autorzy ?? "", s)
                );
            }

            // 3. Pobieramy wyniki
            var ksiazki = await query
                .Select(k => new { 
                    id = k.Id, 
                    tytul = k.Tytul, 
                    autor = k.Autorzy 
                })
                .Take(10)
                .ToListAsync();

            return Json(ksiazki);
        }

        // --- ZAKŁADKA: REJESTRACJA WYPOŻYCZEŃ ---
        [Authorize(Roles = "Bibliotekarz")] 
        [HttpGet]
        public IActionResult RejestracjaWypozyczen()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Bibliotekarz")]
        public async Task<IActionResult> RejestracjaWypozyczen(int idKlienta, List<int> idKsiazek, string czasTrwania)
        {
            if (idKlienta <= 0 || idKsiazek == null || !idKsiazek.Any())
            {
                TempData["ErrorMessage"] = "Błąd: Wybierz klienta i przynajmniej jedną książkę.";
                return RedirectToAction(nameof(RejestracjaWypozyczen));
            }

            // Obliczanie terminu zwrotu (Krok 12 i 14 scenariusza)
            DateTime termin = DateTime.Now.AddDays(14); // Standardowe 2 tygodnie
            if (czasTrwania == "1m") termin = DateTime.Now.AddMonths(1);
            else if (czasTrwania == "2m") termin = DateTime.Now.AddMonths(2);

            // 1. Tworzymy główny rekord (Nagłówek)
            var wypozyczenie = new Wypozyczenie
            {
                KlientId = idKlienta,
                BibliotekarzId = User.Identity?.Name ?? "Nieznany",
                DataWypozyczenia = DateTime.Now,
                TerminZwrotu = termin,
                Status = "Nowe"
            };

            // 2. Dodajemy każdą książkę jako pozycję
            foreach (var idKsiazki in idKsiazek)
            {
                var ksiazka = await _context.Ksiazki.FindAsync(idKsiazki);
                if (ksiazka != null && ksiazka.Status == "Dostępna")
                {
                    ksiazka.Status = "Wypożyczona"; // Krok 14: Zmiana statusu książki

                    wypozyczenie.Pozycje.Add(new WypozyczeniePozycja
                    {
                        KsiazkaId = idKsiazki
                        // DataFaktycznegoZwrotu zostaje null (bo jeszcze nie oddana)
                    });
                }
            }

            _context.Wypozyczenia.Add(wypozyczenie);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Zarejestrowano wypożyczenie książek.";
            return RedirectToAction(nameof(RejestracjaWypozyczen));
        }

        // --- ZAKŁADKA: LISTA WYPOŻYCZEŃ ---
        [Authorize(Roles = "Bibliotekarz,Manager")] // Udostępnione dla Managera
        [HttpGet]
        public IActionResult ListaWypozyczen(string wypozyczajacy, string bibliotekarz, DateTime? dataOd, DateTime? dataDo, string status)
        {
            var query = _wypozyczenia.AsEnumerable();
            bool hasAnyLoans = query.Any();

            if (!string.IsNullOrEmpty(wypozyczajacy))
                query = query.Where(w => w.Wypozyczajacy.Contains(wypozyczajacy, StringComparison.OrdinalIgnoreCase));
            
            if (!string.IsNullOrEmpty(bibliotekarz) && bibliotekarz != "Wszyscy bibliotekarze")
                query = query.Where(w => w.Bibliotekarz == bibliotekarz);
            
            if (!string.IsNullOrEmpty(status) && status != "Wszystkie statusy")
                query = query.Where(w => w.Status == status);
            
            if (dataOd.HasValue)
                query = query.Where(w => w.DataWypozyczenia >= dataOd.Value);
                
            if (dataDo.HasValue)
                query = query.Where(w => w.DataZwrotu <= dataDo.Value);

            var result = query.ToList();

            ViewBag.HasAnyLoans = hasAnyLoans;
            ViewBag.HasFilterResults = result.Any();
            ViewBag.Wypozyczajacy = wypozyczajacy;
            ViewBag.Bibliotekarz = bibliotekarz;
            ViewBag.DataOd = dataOd?.ToString("yyyy-MM-dd");
            ViewBag.DataDo = dataDo?.ToString("yyyy-MM-dd");
            ViewBag.Status = status;

            return View(result);
        }

        // --- AKCJA: PRZEDŁUŻENIE WYPOŻYCZENIA ---
        [Authorize(Roles = "Bibliotekarz,Manager")] // Udostępnione dla Managera
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PrzedluzWypozyczenie(int id)
        {
            var wypozyczenie = _wypozyczenia.FirstOrDefault(w => w.Id == id);
            
            if (wypozyczenie != null)
            {
                wypozyczenie.DataZwrotu = wypozyczenie.DataZwrotu.AddDays(14);
                wypozyczenie.Status = "Przedłużone";
                TempData["SuccessMessage"] = $"Przedłużono wypożyczenie użytkownika {wypozyczenie.Wypozyczajacy}.";
            }

            return RedirectToAction(nameof(ListaWypozyczen));
        }

        // --- AKCJA: ZWROT KSIĄŻEK (Połączona z bazą Entity Framework!) ---
        [Authorize(Roles = "Bibliotekarz,Manager")] // Udostępnione dla Managera
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ZwrotWypozyczenia(int id)
        {
            var wypozyczenie = _wypozyczenia.FirstOrDefault(w => w.Id == id);
            
            if (wypozyczenie != null)
            {
                // Zmieniamy status w widoku wypożyczeń
                wypozyczenie.Status = "Zakończone";
                wypozyczenie.DataZwrotu = DateTime.Now;

                // Aktualizujemy prawdziwą bazę książek (Zwalniamy książkę)
                var ksiazka = await _context.Ksiazki.FirstOrDefaultAsync(k => k.Tytul == wypozyczenie.Ksiazka);
                if (ksiazka != null)
                {
                    ksiazka.Status = "Dostępna";
                    await _context.SaveChangesAsync();
                }
                
                TempData["SuccessMessage"] = $"Użytkownik {wypozyczenie.Wypozyczajacy} dokonał zwrotu książek.";
            }

            return RedirectToAction(nameof(ListaWypozyczen));
        }
    }
}