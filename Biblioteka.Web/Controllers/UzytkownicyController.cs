using Microsoft.AspNetCore.Mvc;
using Biblioteka.Web.Models;
using Biblioteka.Web.Helpers;
using Biblioteka.Web.Data;
using Biblioteka.Web.Data.Entities;

namespace Biblioteka.Web.Controllers
{
    public class UzytkownicyController : Controller
    {
        private readonly BibliotekaDbContext _context;

        public UzytkownicyController(BibliotekaDbContext context)
        {
            _context = context;
        }

        // Widok główny Panelu (Ten z ikoną tarczy)
        public IActionResult Dashboard()
        {
            return View();
        }

        // Lista aktywnych klientów
        public IActionResult Index()
        {
            var users = _context.Uzytkownicy
                .Select(u => new UzytkownikListItemViewModel
                {
                    Login = u.Login,
                    Imie = u.Imie,
                    Nazwisko = u.Nazwisko,
                    Email = u.Email
                })
                .ToList();

            return View(users);
        }

        // Formularz dodawania nowego użytkownika
        public IActionResult Dodaj()
        {
            return View();
        }

        // Akcja odbierająca dane z formularza dodawania
    [HttpPost]
    public IActionResult Dodaj(DodajUzytkownikaViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // sprawdzenie unikalności
        if (_context.Uzytkownicy.Any(u => u.Login == model.Login))
        {
            ModelState.AddModelError("Login", "Ten login już istnieje.");
            return View(model);
        }

        if (_context.Uzytkownicy.Any(u => u.Email == model.Email))
        {
            ModelState.AddModelError("Email", "Ten email już istnieje.");
            return View(model);
        }

        if (_context.Uzytkownicy.Any(u => u.Pesel == model.Pesel))
        {
            ModelState.AddModelError("Pesel", "Ten PESEL już istnieje.");
            return View(model);
        }

        var user = new Uzytkownik
        {
            // Dane z formularza
            Login = model.Login,
            Imie = model.Imie,
            Nazwisko = model.Nazwisko,
            Pesel = model.Pesel,
            DataUrodzenia = model.DataUrodzenia,
            Plec = model.Plec,
            Email = model.Email,
            Telefon = model.Telefon,
            Miejscowosc = model.Miejscowosc,
            KodPocztowy = model.KodPocztowy,
            Ulica = model.Ulica,
            NumerPosesji = model.NumerPosesji,
            NumerLokalu = model.NumerLokalu,

            HasloHash = null,                
            CzyZablokowany = false,         
            BlokadaDo = null,               
            LiczbaBlednychLogowan = 0,      
            CzyZapomniany = false,         
            DataZapomnienia = null,         
            ZapomnianyPrzezId = null         
        };

        _context.Uzytkownicy.Add(user);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

        // Lista zablokowanych użytkowników
        public IActionResult Zapomniani()
        {
            return View();
        }

        // Szczegóły konkretnego klienta
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
