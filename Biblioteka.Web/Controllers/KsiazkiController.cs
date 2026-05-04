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

        // --- ZRK-01: LISTA KSIĄŻEK (Dla Bibliotekarza/Użytkownika) ---
        [HttpGet]
        [Authorize(Roles = "Bibliotekarz,Manager")]
        public async Task<IActionResult> Lista(KsiazkaFiltrViewModel filtr)
        {
            var query = _context.Ksiazki.AsQueryable();

            // FILTR TYTUŁU: Teraz ignoruje wielkość liter
            if (!string.IsNullOrEmpty(filtr.Tytul))
            {
                // Zamieniamy i tytuł w bazie, i szukaną frazę na małe litery
                query = query.Where(k => k.Tytul.ToLower().Contains(filtr.Tytul.ToLower()));
            }

            // Pozostałe filtry (tutaj zazwyczaj wybierasz z listy, więc są dokładne)
            if (!string.IsNullOrEmpty(filtr.WybranyAutor))
                query = query.Where(k => k.Autorzy == filtr.WybranyAutor);

            if (!string.IsNullOrEmpty(filtr.WybranyGatunek))
                query = query.Where(k => k.Gatunek == filtr.WybranyGatunek);

            if (!string.IsNullOrEmpty(filtr.WybraneWydawnictwo))
                query = query.Where(k => k.Wydawnictwo == filtr.WybraneWydawnictwo);

            if (!string.IsNullOrEmpty(filtr.WybranyStatus))
                query = query.Where(k => k.Status == filtr.WybranyStatus);

            // Reszta kodu bez zmian...
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

            return View(filtr);
        }

        // --- ZRK-02: LISTA REJESTRACJI (Dla Managera) ---
        [HttpGet]
        [Authorize(Roles = "Administrator,Manager")] // Tylko dla uprawnionych aktorów
        public async Task<IActionResult> ListaRejestracji(
            string[] autorzy,
            string[] gatunki,
            string tytul,
            string wydawnictwo,
            DateTime? dataOd,
            DateTime? dataDo,
            string[] pracownicy)
        {
            // Scenariusz wyjątku: Nieprawidłowe zaznaczenie zakresu dat
            if (dataOd.HasValue && dataDo.HasValue && dataOd > dataDo)
            {
                ModelState.AddModelError("DataRange", "Data początkowa nie może być późniejsza niż data końcowa.");
            }

            var query = _context.Ksiazki.AsQueryable();

            // --- Logika filtrowania (Scenariusz alternatywny ZRK-02) ---
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
                Id = k.Id,
                Tytul = k.Tytul,
                Autor = k.Autorzy,
                Gatunek = k.Gatunek,
                Wydawnictwo = k.Wydawnictwo,
                DataRejestracji = k.DataRejestracji,
                OsobaRejestrujaca = k.OsobaRejestrujaca
            }).ToListAsync();

            // Przygotowanie list do filtrów wielokrotnego wyboru
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
                Id = istniejaca.Id,
                Tytul = istniejaca.Tytul,
                Autor = istniejaca.Autorzy,
                Gatunek = istniejaca.Gatunek
            };

            PrepareGenreList();
            return View("Zarejestruj", nowyEgzemplarz);
        }

        // --- ZRK-01: ZAREJESTRUJ (Zaktualizowane o metadane dla ZRK-02) ---
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
                    Tytul = model.Tytul,
                    Autorzy = model.Autor,
                    Gatunek = model.Gatunek,
                    Wydawnictwo = model.Wydawnictwo,
                    RokWydania = model.RokWydania ?? DateTime.Now.Year,
                    LiczbaStron = model.LiczbaStron ?? 0,
                    Cena = model.Cena ?? 0,
                    LiczbaSztuk = model.LiczbaSztuk ?? 0,
                    Opis = model.Opis ?? "",
                    Status = "Dostępna",

                    // --- Metadane dla ZRK-02 ---
                    DataRejestracji = DateTime.Now,
                    OsobaRejestrujaca = User.Identity?.Name ?? "System"
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
                Id = ksiazka.Id,
                Tytul = ksiazka.Tytul,
                Autor = ksiazka.Autorzy,
                Gatunek = ksiazka.Gatunek,
                Wydawnictwo = ksiazka.Wydawnictwo,
                RokWydania = ksiazka.RokWydania,
                LiczbaStron = ksiazka.LiczbaStron,
                Cena = ksiazka.Cena,
                LiczbaSztuk = ksiazka.LiczbaSztuk,
                Opis = ksiazka.Opis,
                Status = ksiazka.Status,
                DataRejestracji = ksiazka.DataRejestracji,
                OsobaRejestrujaca = ksiazka.OsobaRejestrujaca
            };

            return View(model);
        }

        private void PrepareGenreList()
        {
            ViewBag.Gatunki = new List<SelectListItem>
            {
                new SelectListItem { Value = "Fantastyka", Text = "Fantastyka" },
                new SelectListItem { Value = "Kryminał", Text = "Kryminał" },
                new SelectListItem { Value = "Literatura piękna", Text = "Literatura piękna" },
                new SelectListItem { Value = "Nauka", Text = "Nauka" },
                new SelectListItem { Value = "Thriller", Text = "Thriller" },
                new SelectListItem { Value = "Biografia", Text = "Biografia" },
                new SelectListItem { Value = "Historyczna", Text = "Historyczna" }
            };
        }
    }
}