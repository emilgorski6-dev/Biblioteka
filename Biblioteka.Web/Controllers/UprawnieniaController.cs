using Microsoft.AspNetCore.Mvc;
using Biblioteka.Web.Models;
using System.Collections.Generic;
using System.Linq;

// UWAGA 1: Jeśli Twój DbContext znajduje się w innym folderze (np. Data), odkomentuj poniższą linię i dopasuj nazwę
// using Biblioteka.Web.Data; 

namespace Biblioteka.Web.Controllers
{
    public class UprawnieniaController : Controller
    {
        // =========================================================================
        // KROK 1: ODKOMENTUJ TEN BLOK, ABY WSTRZYKNĄĆ SWOJĄ BAZĘ DANYCH
        // (Upewnij się, że nazwa 'BibliotekaDbContext' zgadza się z Twoim projektem)
        // =========================================================================
        
        /*
        private readonly BibliotekaDbContext _context;

        public UprawnieniaController(BibliotekaDbContext context)
        {
            _context = context;
        }
        */

        // ==========================================
        // AKCJA 1: GŁÓWNA LISTA UPRAWNIEŃ (Index)
        // ==========================================
        public IActionResult Index()
        {
            // =========================================================================
            // KROK 2: ODKOMENTUJ TO, ABY POBIERAĆ PRAWDZIWE LICZBY Z BAZY
            // (Dostosuj "Rola" do kolumny w swojej tabeli Uzytkownicy)
            // =========================================================================
            
            /*
            int countSelect = _context.Uzytkownicy.Count(u => u.Rola == "Administrator" || u.Rola == "Bibliotekarz" || u.Rola == "Czytelnik");
            int countInsert = _context.Uzytkownicy.Count(u => u.Rola == "Administrator" || u.Rola == "Bibliotekarz");
            int countUpdate = _context.Uzytkownicy.Count(u => u.Rola == "Administrator" || u.Rola == "Bibliotekarz");
            int countDelete = _context.Uzytkownicy.Count(u => u.Rola == "Administrator");
            int countAlter  = _context.Uzytkownicy.Count(u => u.Rola == "Administrator");
            int countAll    = _context.Uzytkownicy.Count(u => u.Rola == "Administrator");
            */

            // --- WERSJA TYMCZASOWA (Do momentu odkomentowania kodu wyżej usuń ten blok) ---
            int countSelect = 2; 
            int countInsert = 4;
            int countUpdate = 3;
            int countDelete = 2;
            int countAlter = 1;
            int countAll = 2;
            // -------------------------------------------------------------------------------

            var listaUprawnien = new List<UprawnienieItemViewModel>
            {
                new UprawnienieItemViewModel { Id = "SELECT", Nazwa = "SELECT", Opis = "Uprawnienie do odczytu danych z tabel", BadgeClass = "badge-select", LiczbaUzytkownikow = countSelect },
                new UprawnienieItemViewModel { Id = "INSERT", Nazwa = "INSERT", Opis = "Uprawnienie do dodawania nowych rekordów", BadgeClass = "badge-insert", LiczbaUzytkownikow = countInsert },
                new UprawnienieItemViewModel { Id = "UPDATE", Nazwa = "UPDATE", Opis = "Uprawnienie do modyfikacji istniejących danych", BadgeClass = "badge-update", LiczbaUzytkownikow = countUpdate },
                new UprawnienieItemViewModel { Id = "DELETE", Nazwa = "DELETE", Opis = "Uprawnienie do usuwania rekordów", BadgeClass = "badge-delete", LiczbaUzytkownikow = countDelete },
                new UprawnienieItemViewModel { Id = "ALTER", Nazwa = "ALTER", Opis = "Uprawnienie do modyfikacji struktury tabel", BadgeClass = "badge-alter", LiczbaUzytkownikow = countAlter },
                new UprawnienieItemViewModel { Id = "ALL", Nazwa = "ALL PRIVILEGES", Opis = "Pełne uprawnienia do wszystkich operacji", BadgeClass = "badge-all", LiczbaUzytkownikow = countAll }
            };

            return View(listaUprawnien);
        }

        // ==========================================
        // AKCJA 2: SZCZEGÓŁY UPRAWNIENIA (Szczegoly)
        // ==========================================
        public IActionResult Szczegoly(string id)
        {
            string uprawnienie = string.IsNullOrEmpty(id) ? "SELECT" : id.ToUpper();

            var model = new UprawnienieSzczegolyViewModel
            {
                NazwaUprawnienia = uprawnienie
            };

            // =========================================================================
            // KROK 3: ODKOMENTUJ TO, ABY POBIERAĆ PRAWDZIWYCH UŻYTKOWNIKÓW Z BAZY
            // =========================================================================
            
            /*
            // Przykład pobierania administratorów dla uprawnienia DELETE:
            if (uprawnienie == "DELETE" || uprawnienie == "ALTER" || uprawnienie == "ALL PRIVILEGES")
            {
                var userzyZBazy = _context.Uzytkownicy.Where(u => u.Rola == "Administrator").ToList();
                foreach (var u in userzyZBazy)
                {
                    model.Uzytkownicy.Add(new UzytkownikZUprawnieniem { 
                        Login = u.Login, 
                        ImieNazwisko = $"{u.Imie} {u.Nazwisko}", 
                        Rola = u.Rola 
                    });
                }
            }
            // ... i tak dalej dla innych uprawnień
            */

            // --- WERSJA TYMCZASOWA (Do momentu odkomentowania kodu wyżej) ---
            if (uprawnienie == "SELECT")
            {
                model.Uzytkownicy.Add(new UzytkownikZUprawnieniem { Login = "jkowalski", ImieNazwisko = "Jan Kowalski", Rola = "Administrator" });
                model.Uzytkownicy.Add(new UzytkownikZUprawnieniem { Login = "anowak", ImieNazwisko = "Anna Nowak", Rola = "Administrator" });
            }
            else if (uprawnienie == "INSERT")
            {
                model.Uzytkownicy.Add(new UzytkownikZUprawnieniem { Login = "pwisniewski", ImieNazwisko = "Piotr Wiśniewski", Rola = "Bibliotekarz" });
                model.Uzytkownicy.Add(new UzytkownikZUprawnieniem { Login = "mlewandowska", ImieNazwisko = "Magdalena Lewandowski", Rola = "Bibliotekarz" });
            }
            else if (uprawnienie == "UPDATE")
            {
                model.Uzytkownicy.Add(new UzytkownikZUprawnieniem { Login = "pwisniewski", ImieNazwisko = "Piotr Wiśniewski", Rola = "Bibliotekarz" });
            }
            else if (uprawnienie == "DELETE")
            {
                model.Uzytkownicy.Add(new UzytkownikZUprawnieniem { Login = "jkowalski", ImieNazwisko = "Jan Kowalski", Rola = "Administrator" });
            }
            else if (uprawnienie == "ALL" || uprawnienie == "ALL PRIVILEGES")
            {
                model.NazwaUprawnienia = "ALL PRIVILEGES"; 
                model.Uzytkownicy.Add(new UzytkownikZUprawnieniem { Login = "jkowalski", ImieNazwisko = "Jan Kowalski", Rola = "Administrator" });
            }
            // -------------------------------------------------------------------------------

            return View("uprawnienia", model);
        }
    }
}