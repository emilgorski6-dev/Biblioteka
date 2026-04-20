using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblioteka.Web.Models;
using Biblioteka.Web.Data;
using Biblioteka.Web.Data.Entities; // Dodane dla dostępu do encji Uzytkownik
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
                    // Nakładamy kolejne filtry - każdy musi zostać spełniony
                    userQuery = userQuery.Where(u => u.Uprawnienia.Any(r => r.Nazwa == role));
                }

                // Pobieramy wyniki tylko jeśli wybrano jakieś role
                ViewBag.FilteredUsers = userQuery.ToList();
            }
            else
            {
                // Jeśli nic nie zaznaczono, przekazujemy pustą listę (lub wszystkich, zależnie od preferencji)
                ViewBag.FilteredUsers = new List<Uzytkownik>();
            }

            // Przekazujemy wybrane role z powrotem, aby checkboxy zostały zaznaczone po odświeżeniu
            ViewBag.SelectedRoles = selectedRoles ?? new string[0];

            return View(listaUprawnien);
        }

        public IActionResult Szczegoly(int id)
        {
            // Pobieramy Uprawnienie i jego użytkowników
            var uprawnienie = _context.Uprawnienia
                .Include(p => p.Uzytkownicy)
                .FirstOrDefault(p => p.Id == id);

            if (uprawnienie == null) return NotFound();

            // Filtrowanie i mapowanie w pamięci (Memory) dla SQLite
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

            return View("uprawnienia", model);
        }
    }
}