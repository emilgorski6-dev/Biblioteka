using Microsoft.AspNetCore.Mvc;
using Biblioteka.Web.Data;
using Biblioteka.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteka.Web.Controllers
{
    // ZRK-02: Aktor to Manager biblioteki (zazwyczaj rola Administrator lub Manager)
    [Authorize(Roles = "Administrator,Manager")]
    public class ManagerController : Controller
    {
        private readonly BibliotekaDbContext _context;

        public ManagerController(BibliotekaDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Widok listy rejestracji książek (ZRK-02)
        [HttpGet]
        public async Task<IActionResult> Rejestracje(
            string[] autorzy,
            string[] gatunki,
            string tytul,
            string wydawnictwo,
            DateTime? dataOd,
            DateTime? dataDo,
            string[] pracownicy)
        {
            // 1. Sprawdzenie wyjątku: Nieprawidłowy zakres dat
            if (dataOd.HasValue && dataDo.HasValue && dataOd > dataDo)
            {
                ModelState.AddModelError("DataRange", "Data początkowa nie może być późniejsza niż data końcowa.");
                // Przekazujemy błąd do widoku, ale kontynuujemy, by manager widział formularz
            }

            // 2. Budowanie zapytania (Scenariusz główny i alternatywny)
            var query = _context.Ksiazki.AsQueryable();

            // Flaga sprawdzająca, czy jakiekolwiek filtrowanie jest aktywne
            bool isFilterActive = (autorzy?.Length > 0 || gatunki?.Length > 0 || !string.IsNullOrEmpty(tytul) ||
                                   !string.IsNullOrEmpty(wydawnictwo) || dataOd.HasValue || dataDo.HasValue || pracownicy?.Length > 0);

            // 3. Aplikowanie filtrów (Scenariusz alternatywny)
            // --- Logika filtrowania z rozpoznawaniem małych/wielkich liter ---
            if (!string.IsNullOrEmpty(tytul))
            {
                query = query.Where(k => k.Tytul.ToLower().Contains(tytul.ToLower()));
            }

            if (!string.IsNullOrEmpty(wydawnictwo))
            {
                query = query.Where(k => k.Wydawnictwo.ToLower().Contains(wydawnictwo.ToLower()));
            }

            if (autorzy?.Length > 0)
                query = query.Where(k => autorzy.Contains(k.Autorzy));

            if (gatunki?.Length > 0)
                query = query.Where(k => gatunki.Contains(k.Gatunek));

            if (pracownicy?.Length > 0)
                query = query.Where(k => pracownicy.Contains(k.OsobaRejestrujaca));

            if (dataOd.HasValue)
                query = query.Where(k => k.DataRejestracji >= dataOd.Value);

            if (dataDo.HasValue)
                query = query.Where(k => k.DataRejestracji <= dataDo.Value);

            // 4. Mapowanie na ViewModel
            var historia = await query
                .OrderByDescending(k => k.DataRejestracji) // Najnowsze na górze
                .Select(k => new KsiazkaListaViewModel
                {
                    Id = k.Id,
                    Tytul = k.Tytul,
                    Autor = k.Autorzy,
                    Gatunek = k.Gatunek,
                    Wydawnictwo = k.Wydawnictwo,
                    DataRejestracji = k.DataRejestracji,
                    OsobaRejestrujaca = k.OsobaRejestrujaca
                })
                .ToListAsync();

            // 5. Obsługa komunikatów o braku danych (Scenariusze wyjątków)
            if (!historia.Any())
            {
                ViewBag.EmptyMessage = isFilterActive
                    ? "Brak rejestracji spełniających kryteria filtrowania"
                    : "Nie zarejestrowano jeszcze żadnych książek";
            }

            // 6. Przekazanie danych do filtrów (Distinct listy dla UI)
            ViewBag.DostepniAutorzy = await _context.Ksiazki.Select(k => k.Autorzy).Distinct().ToListAsync();
            ViewBag.DostepneGatunki = await _context.Ksiazki.Select(k => k.Gatunek).Distinct().ToListAsync();
            ViewBag.Pracownicy = await _context.Ksiazki.Select(k => k.OsobaRejestrujaca).Distinct().ToListAsync();
            ViewBag.IsFilterActive = isFilterActive;

            return View(historia);
        }
    }
}