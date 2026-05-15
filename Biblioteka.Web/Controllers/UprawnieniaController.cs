using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblioteka.Web.Models;
using Biblioteka.Web.Data;
using Biblioteka.Web.Data.Entities;
using System.Linq;
using System.Collections.Generic;

namespace Biblioteka.Web.Controllers
{
    public class UprawnieniaController : Controller
    {
        private readonly BibliotekaDbContext _context;

        public UprawnieniaController(BibliotekaDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string[] selectedRoles)
        {
            // 1. Pobieramy listę uprawnień do wyświetlenia kart i checkboxów
            var listaDoWyswietlenia = _context.Uprawnienia
                .Select(uprawnienie => new UprawnienieItemViewModel
                {
                    Id = uprawnienie.Id,
                    Nazwa = uprawnienie.Nazwa,
                    Opis = uprawnienie.Opis ?? string.Empty,
                    // Liczymy użytkowników, którzy nie zostali "zapomniani" (RODO)
                    LiczbaUzytkownikow = uprawnienie.Uzytkownicy.Count(u => !u.CzyZapomniany)
                }).ToList();

            List<Uzytkownik> filteredUsers = new List<Uzytkownik>();

            if (selectedRoles != null && selectedRoles.Length > 0)
            {
                filteredUsers = _context.Uzytkownicy
                    .Include(u => u.Uprawnienia)
                    .Where(u => !u.CzyZapomniany)
                    .AsEnumerable() // Przejście na operacje w pamięci (LINQ to Objects)
                    .Where(u => selectedRoles.All(role =>
                        u.Uprawnienia.Any(up => up.Nazwa == role)))
                    .ToList();
            }

            ViewBag.FilteredUsers = filteredUsers;
            ViewBag.SelectedRoles = selectedRoles ?? new string[0];

            return View(listaDoWyswietlenia);
        }

        // Metoda Szczegoly bez zmian...
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

            return View("uprawnienia", model);
        }
    }
}