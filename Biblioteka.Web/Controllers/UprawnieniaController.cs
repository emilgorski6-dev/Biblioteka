using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Biblioteka.Web.Models;
using Biblioteka.Web.Data;
using System.Linq;

namespace Biblioteka.Web.Controllers
{
    public class UprawnieniaController : Controller
    {
        private readonly BibliotekaDbContext _bazaDanych;

        public UprawnieniaController(BibliotekaDbContext context)
        {
            _bazaDanych = context;
        }

        // AKCJA 1: Lista ról (Index)
        public IActionResult Index()
        {
            var listaDoWyswietlenia = _bazaDanych.Uprawnienia.Select(uprawnienie => new UprawnienieItemViewModel
            {
                Id = uprawnienie.Id,
                Nazwa = uprawnienie.Nazwa,
                Opis = uprawnienie.Opis ?? string.Empty,
                // Liczymy tylko tych, którzy nie są zapomniani
                LiczbaUzytkownikow = uprawnienie.Uzytkownicy.Count(u => !u.CzyZapomniany)
            }).ToList();

            return View(listaDoWyswietlenia);
        }

        // AKCJA 2: Szczegóły roli (Szczegoly)
        public IActionResult Szczegoly(int id)
        {
            // Pobieramy uprawnienie wraz z listą użytkowników
            var uprawnienie = _bazaDanych.Uprawnienia
                .Include(u => u.Uzytkownicy)
                .FirstOrDefault(u => u.Id == id);

            if (uprawnienie == null) return NotFound();

            var model = new UprawnienieSzczegolyViewModel
            {
                NazwaUprawnienia = uprawnienie.Nazwa.ToUpper(),
                Uzytkownicy = uprawnienie.Uzytkownicy
                    .Where(u => !u.CzyZapomniany) // Tylko aktywni użytkownicy
                    .Select(u => new UzytkownikZUprawnieniem
                    {
                        Login = u.Login,
                        ImieNazwisko = $"{u.Imie} {u.Nazwisko}",
                        Rola = uprawnienie.Nazwa
                    }).ToList()
            };

            // Zauważ, że zwracasz widok o nazwie "uprawnienia"
            return View("uprawnienia", model);
        }
    }
}