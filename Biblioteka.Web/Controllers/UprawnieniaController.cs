using Microsoft.AspNetCore.Mvc;
using Biblioteka.Web.Models;
using System.Collections.Generic;
using System.Linq;
// using Biblioteka.Web.Data; // Odkomentuj, gdy podepniesz bazę

namespace Biblioteka.Web.Controllers
{
    public class UprawnieniaController : Controller
    {
        // =========================================================================
        // KROK 1: WSTRZYKNIĘCIE BAZY DANYCH (Odkomentuj, gdy będziesz gotowy)
        // =========================================================================
        /*
        private readonly BibliotekaDbContext _context;

        public UprawnieniaController(BibliotekaDbContext context)
        {
            _context = context;
        }
        */

        // ==========================================
        // AKCJA 1: GŁÓWNA LISTA RÓL (Index)
        // ==========================================
        public IActionResult Index()
        {
            // SYMULACJA ZLICZANIA (Docelowo użyj: _context.Uzytkownicy.Count(u => u.Rola == "Nazwa"))
            int countAdmin = 2;
            int countBibliotekarz = 4;
            int countManager = 1;
            int countKlient = 150;

            var listaRol = new List<UprawnienieItemViewModel>
            {
                new UprawnienieItemViewModel { 
                    Id = "Administrator", 
                    Nazwa = "ADMINISTRATOR", 
                    Opis = "Pełny dostęp do zarządzania systemem, bazą danych i uprawnieniami użytkowników.", 
                    BadgeClass = "badge-all", 
                    LiczbaUzytkownikow = countAdmin 
                },
                new UprawnienieItemViewModel { 
                    Id = "Bibliotekarz", 
                    Nazwa = "BIBLIOTEKARZ", 
                    Opis = "Zarządzanie księgozbiorem, obsługą wypożyczeń, zwrotów oraz katalogowaniem pozycji.", 
                    BadgeClass = "badge-insert", 
                    LiczbaUzytkownikow = countBibliotekarz 
                },
                new UprawnienieItemViewModel { 
                    Id = "Manager", 
                    Nazwa = "MANAGER", 
                    Opis = "Dostęp do statystyk, raportów finansowych i zarządzania kadrą biblioteczną.", 
                    BadgeClass = "badge-alter", 
                    LiczbaUzytkownikow = countManager 
                },
                new UprawnienieItemViewModel { 
                    Id = "Klient", 
                    Nazwa = "KLIENT", 
                    Opis = "Podstawowy dostęp: przeglądanie katalogu, rezerwacje online i zarządzanie własnym profilem.", 
                    BadgeClass = "badge-select", 
                    LiczbaUzytkownikow = countKlient 
                }
            };

            return View(listaRol);
        }

        // ==========================================
        // AKCJA 2: LISTA UŻYTKOWNIKÓW DLA ROLI (Szczegoly)
        // ==========================================
        public IActionResult Szczegoly(string id)
        {
            // Zabezpieczenie przed pustym ID
            string rola = string.IsNullOrEmpty(id) ? "Klient" : id;

            var model = new UprawnienieSzczegolyViewModel
            {
                NazwaUprawnienia = rola.ToUpper()
            };

            // SYMULACJA POBIERANIA UŻYTKOWNIKÓW Z BAZY
            // Docelowo: var userzy = _context.Uzytkownicy.Where(u => u.Rola == rola).ToList();

            if (rola.Equals("Administrator", System.StringComparison.OrdinalIgnoreCase))
            {
                model.Uzytkownicy.Add(new UzytkownikZUprawnieniem { Login = "admin", ImieNazwisko = "Emil Górski", Rola = "Administrator" });
                model.Uzytkownicy.Add(new UzytkownikZUprawnieniem { Login = "jkowalski", ImieNazwisko = "Jan Kowalski", Rola = "Administrator" });
            }
            else if (rola.Equals("Bibliotekarz", System.StringComparison.OrdinalIgnoreCase))
            {
                model.Uzytkownicy.Add(new UzytkownikZUprawnieniem { Login = "pwisniewski", ImieNazwisko = "Piotr Wiśniewski", Rola = "Bibliotekarz" });
                model.Uzytkownicy.Add(new UzytkownikZUprawnieniem { Login = "mlewandowska", ImieNazwisko = "Magdalena Lewandowski", Rola = "Bibliotekarz" });
            }
            else if (rola.Equals("Manager", System.StringComparison.OrdinalIgnoreCase))
            {
                model.Uzytkownicy.Add(new UzytkownikZUprawnieniem { Login = "anowak", ImieNazwisko = "Anna Nowak", Rola = "Manager" });
            }
            else if (rola.Equals("Klient", System.StringComparison.OrdinalIgnoreCase))
            {
                model.Uzytkownicy.Add(new UzytkownikZUprawnieniem { Login = "mwojcik", ImieNazwisko = "Maria Wójcik", Rola = "Klient" });
                model.Uzytkownicy.Add(new UzytkownikZUprawnieniem { Login = "tzielinski", ImieNazwisko = "Tomasz Zieliński", Rola = "Klient" });
            }

            // Zwraca widok uprawnienia.cshtml z listą osób przypisanych do danej roli
            return View("uprawnienia", model);
        }
    }
}