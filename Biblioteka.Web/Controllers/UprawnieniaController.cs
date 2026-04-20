using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblioteka.Web.Models;
using Biblioteka.Web.Data;
using Biblioteka.Web.Data.Entities;
using System.Linq;

namespace Biblioteka.Web.Controllers
{
    public class UprawnieniaController : Controller
    {
        private readonly BibliotekaDbContext _context;

        public UprawnieniaController(BibliotekaDbContext context)
        {
            _context = context;
        }

        // Zaktualizowana metoda Index obsługująca filtrowanie
        public IActionResult Index(string[] selectedRoles)
        {
            // 1. Pobieramy listę wszystkich uprawnień, aby wyświetlić "chipsy" w widoku
            var listaUprawnien = _context.Uprawnienia
                .Select(uprawnienie => new UprawnienieItemViewModel
                {
                    Id = uprawnienie.Id,
                    Nazwa = uprawnienie.Nazwa,
                    Opis = uprawnienie.Opis ?? string.Empty,
                    LiczbaUzytkownikow = uprawnienie.Uzytkownicy.Count(u => !u.CzyZapomniany)
                }).ToList();

            // 2. Inicjujemy zapytanie o użytkowników
            var userQuery = _context.Uzytkownicy
                .Include(u => u.Uprawnienia)
                .Where(u => !u.CzyZapomniany)
                .AsQueryable();

            // 3. Logika filtrowania (AND) - Użytkownik musi posiadać KAŻDĄ z wybranych ról
            if (selectedRoles != null && selectedRoles.Length > 0)
            {
                foreach (var role in selectedRoles)
                {
                    userQuery = userQuery.Where(u => u.Uprawnienia.Any(r => r.Nazwa == role));
                }

                ViewBag.FilteredUsers = userQuery.ToList();
            }
            else
            {
                ViewBag.FilteredUsers = new List<Uzytkownik>();
            }

            ViewBag.SelectedRoles = selectedRoles ?? new string[0];

            return View(listaUprawnien);
        }

        public IActionResult Szczegoly(int id)
        {
            var uprawnienie = _context.Uprawnienia
                .Include(p => p.Uzytkownicy)
                .FirstOrDefault(p => p.Id == id);

            if (uprawnienie == null) return NotFound();

            var model = new UprawnienieSzczegolyViewModel
            {
                NazwaUprawnienia = uprawnienie.Nazwa.ToUpper(),
                Uzytkownicy = uprawnienie.Uzytkownicy
                    .Where(u => !u.CzyZapomniany)
                    .Select(u => new UzytkownikZUprawnieniem
                    {
                        Login = u.Login,
                        ImieNazwisko = $"{u.Imie} {u.Nazwisko}",
                        Rola = uprawnienie.Nazwa
                    }).ToList()
            };

            if (!model.Uzytkownicy.Any())
            {
                ViewData["Message"] = "Brak użytkowników o wybranym uprawnieniu";
            }

            return View("uprawnienia", model);
        }
    }
}