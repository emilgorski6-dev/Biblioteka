using Microsoft.AspNetCore.Mvc;
using Biblioteka.Web.Models;
using Biblioteka.Web.Helpers;


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
            return View();
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
        public IActionResult Szczegoly(int id)
        {
            return View();
        }
    }
}