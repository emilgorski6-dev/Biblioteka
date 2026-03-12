using Microsoft.AspNetCore.Mvc;
using Biblioteka.Web.Models;
using Biblioteka.Web.Helpers;

// TODO:
// 1. sprawdzić czy login istnieje
// 2. sprawdzić czy PESEL istnieje
// 3. zapisać użytkownika do bazy

namespace Biblioteka.Web.Controllers
{
    public class UzytkownicyController : Controller
    {
        // Widok główny Panelu (Ten z ikoną tarczy)
        // GET: /Uzytkownicy/Dashboard
        public IActionResult Dashboard()
        {
            return View();
        }

        // Lista aktywnych klientów
        // GET: /Uzytkownicy/Index
        public IActionResult Index()
        {
            var users = new List<UzytkownikListItemViewModel>
            {
                new UzytkownikListItemViewModel
                {
                    Login = "jkowalski",
                    Imie = "Jan",
                    Nazwisko = "Kowalski",
                    Email = "jan.kowalski@example.com",
                    Status = "Aktywny"
                },
                new UzytkownikListItemViewModel
                {
                    Login = "pwisniewski",
                    Imie = "Piotr",
                    Nazwisko = "Wiśniewski",
                    Email = "p.wisniewski@example.com",
                    Status = "Aktywny"
                },
                new UzytkownikListItemViewModel
                {
                    Login = "akowalczyk",
                    Imie = "Anna",
                    Nazwisko = "Kowalczyk",
                    Email = "anna.kowalczyk@example.com",
                    Status = "Aktywny"
                },
                new UzytkownikListItemViewModel
                {
                    Login = "mnowak",
                    Imie = "Marek",
                    Nazwisko = "Nowak",
                    Email = "marek.nowak@example.com",
                    Status = "Zablokowany"
                },
                new UzytkownikListItemViewModel
                {
                    Login = "jzielinski",
                    Imie = "Jakub",
                    Nazwisko = "Zieliński",
                    Email = "jakub.zielinski@example.com",
                    Status = "Nieaktywny"
                }
            };

            return View(users);
        }

        // Formularz dodawania nowego użytkownika
        // GET: /Uzytkownicy/Dodaj
        public IActionResult Dodaj()
        {
            return View();
        }

        // Akcja odbierająca dane z formularza dodawania
        [HttpPost]
        public IActionResult Dodaj(DodajUzytkownikaViewModel model) // W miejsce 'object' wejdzie Twój Model
        {
            if (!PeselValidator.CzyPeselJestPoprawny(model.Pesel, model.DataUrodzenia, model.Plec))
            {
                ModelState.AddModelError("Pesel", "Nieprawidłowy numer PESEL");
            }
            if (!EmailValidator.IsValid(model.Email))
            {
                ModelState.AddModelError("Email", "Nieprawidłowy adres email.");
            }

            if (!PhoneValidator.IsValid(model.Telefon))
            {
                ModelState.AddModelError("Telefon", "Nieprawidłowy numer telefonu.");
            }

            if (!BirthDateValidator.IsValid(model.DataUrodzenia))
            {
                ModelState.AddModelError("DataUrodzenia", "Nieprawidłowa data urodzenia.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // tutaj później zapis do bazy

            return RedirectToAction("Index");
        }

        // Lista zablokowanych użytkowników
        // GET: /Uzytkownicy/Zapomniani
        public IActionResult Zapomniani()
        {
            return View();
        }

        // Szczegóły konkretnego klienta
        // GET: /Uzytkownicy/Szczegoly/5
        public IActionResult Szczegoly(string login)
        {
            var user = new
            {
                Login = login,
                Imie = "Jan",
                Nazwisko = "Kowalski",
                Email = "jan.kowalski@example.com",
                Telefon = "123456789"
            };

            return View(user);
        }
    }
}