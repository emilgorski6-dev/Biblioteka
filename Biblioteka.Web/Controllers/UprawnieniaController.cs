using Microsoft.AspNetCore.Mvc;
using Biblioteka.Web.Models;
using System.Collections.Generic;
using System.Linq;
using Biblioteka.Web.Data;

namespace Biblioteka.Web.Controllers
{
    public class UprawnieniaController : Controller
    {
        private readonly BibliotekaDbContext _bazaDanych;

        public UprawnieniaController(BibliotekaDbContext context)
        {
            _bazaDanych = context;
        }

        // ==========================================
        // AKCJA 1: GŁÓWNA LISTA RÓL (Index)
        // ==========================================
        public IActionResult Index()
        {
            var listaDoWyswietlenia = _bazaDanych.Uprawnienia.Select(uprawnienie => new UprawnienieItemViewModel
            {
                Id = uprawnienie.Id,
                Nazwa = uprawnienie.Nazwa,
                Opis = uprawnienie.Opis ?? string.Empty,
                LiczbaUzytkownikow = uprawnienie.Uzytkownicy.Count()
            }).ToList();

            return View(listaDoWyswietlenia);   
        }

        // ==========================================
        // AKCJA 2: LISTA UŻYTKOWNIKÓW DLA ROLI (Szczegoly)
        // ==========================================
        public IActionResult Szczegoly(int id)
        {
            // 1. Szukamy w bazie uprawnienia o tym ID, aby poznać jego nazwę
            var uprawnienie = _bazaDanych.Uprawnienia.FirstOrDefault(u => u.Id == id);

            // 2. Jeśli nie znaleziono w bazie, ustawiamy domyślną nazwę (np. Klient)
            string nazwaRoli = uprawnienie?.Nazwa ?? "Klient";

            var model = new UprawnienieSzczegolyViewModel
            {
                NazwaUprawnienia = nazwaRoli.ToUpper()
            };

            // 3. Sprawdzamy nazwę roli (którą pobraliśmy z bazy), aby dodać testowych użytkowników
            if (nazwaRoli.Equals("Administrator", System.StringComparison.OrdinalIgnoreCase))
            {
                model.Uzytkownicy.Add(new UzytkownikZUprawnieniem { Login = "admin", ImieNazwisko = "Emil Górski", Rola = "Administrator" });
                model.Uzytkownicy.Add(new UzytkownikZUprawnieniem { Login = "jkowalski", ImieNazwisko = "Jan Kowalski", Rola = "Administrator" });
            }
            else if (nazwaRoli.Equals("Bibliotekarz", System.StringComparison.OrdinalIgnoreCase))
            {
                model.Uzytkownicy.Add(new UzytkownikZUprawnieniem { Login = "pwisniewski", ImieNazwisko = "Piotr Wiśniewski", Rola = "Bibliotekarz" });
                model.Uzytkownicy.Add(new UzytkownikZUprawnieniem { Login = "mlewandowska", ImieNazwisko = "Magdalena Lewandowski", Rola = "Bibliotekarz" });
            }
            else if (nazwaRoli.Equals("Manager", System.StringComparison.OrdinalIgnoreCase))
            {
                model.Uzytkownicy.Add(new UzytkownikZUprawnieniem { Login = "anowak", ImieNazwisko = "Anna Nowak", Rola = "Manager" });
            }
            else if (nazwaRoli.Equals("Klient", System.StringComparison.OrdinalIgnoreCase))
            {
                model.Uzytkownicy.Add(new UzytkownikZUprawnieniem { Login = "mwojcik", ImieNazwisko = "Maria Wójcik", Rola = "Klient" });
                model.Uzytkownicy.Add(new UzytkownikZUprawnieniem { Login = "tzielinski", ImieNazwisko = "Tomasz Zieliński", Rola = "Klient" });
            }

            return View("uprawnienia", model);
        }
    }
}