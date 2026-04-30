using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblioteka.Web.Models;
using Biblioteka.Web.Data;
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

        public IActionResult Index()
        {
            var listaDoWyswietlenia = _context.Uprawnienia
                .Select(uprawnienie => new UprawnienieItemViewModel
                {
                    Id = uprawnienie.Id,
                    Nazwa = uprawnienie.Nazwa,
                    Opis = uprawnienie.Opis ?? string.Empty,
                    LiczbaUzytkownikow = uprawnienie.Uzytkownicy.Count(u => !u.CzyZapomniany)
                }).ToList();

            return View(listaDoWyswietlenia);
        }

        public IActionResult Szczegoly(int id)
        {
            // 1. Pobieramy Uprawnienie i jego użytkowników (podstawowy Include działa w SQLite)
            var uprawnienie = _context.Uprawnienia
                .Include(p => p.Uzytkownicy)
                .FirstOrDefault(p => p.Id == id);

            if (uprawnienie == null) return NotFound();

            // 2. Filtrowanie i mapowanie robimy w pamięci (Memory), co rozwiązuje problem SQLite
            var model = new UprawnienieSzczegolyViewModel
            {
                NazwaUprawnienia = uprawnienie.Nazwa.ToUpper(),
                Uzytkownicy = uprawnienie.Uzytkownicy
                    .Where(u => !u.CzyZapomniany) // Filtrujemy w C#
                    .Select(u => new UzytkownikZUprawnieniem
                    {
                        Login = u.Login,
                        ImieNazwisko = $"{u.Imie} {u.Nazwisko}",
                        Rola = uprawnienie.Nazwa
                    }).ToList()
            };

            // Zwracamy widok
            return View("uprawnienia", model);
        }
    }
}