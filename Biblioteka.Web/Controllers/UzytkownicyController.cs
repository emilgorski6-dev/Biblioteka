using Microsoft.AspNetCore.Mvc;
using Biblioteka.Web.Models;
using Biblioteka.Web.Helpers;
using Biblioteka.Web.Data;
using Biblioteka.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Biblioteka.Web.Controllers
{
    public class UzytkownicyController : Controller
    {
        private readonly BibliotekaDbContext _context;

        public UzytkownicyController(BibliotekaDbContext context)
        {
            _context = context;
        }

        // --- ZU-02 i ZU-03: Lista aktywnych klientów z funkcją wyszukiwania ---
        public IActionResult Index(string searchLogin, string searchName, string searchPesel)
        {
            var query = _context.Uzytkownicy
                .Where(u => u.CzyZapomniany == false)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchLogin))
                query = query.Where(u => u.Login.Contains(searchLogin));

            if (!string.IsNullOrEmpty(searchName))
                query = query.Where(u => u.Imie.Contains(searchName) || u.Nazwisko.Contains(searchName));

            if (!string.IsNullOrEmpty(searchPesel))
                query = query.Where(u => u.Pesel.Contains(searchPesel));

            var users = query
                .Select(u => new UzytkownikListItemViewModel
                {
                    Login = u.Login,
                    Imie = u.Imie,
                    Nazwisko = u.Nazwisko,
                    Email = u.Email,
                    Pesel = u.Pesel
                })
                .ToList();

            ViewBag.CurrentLogin = searchLogin;
            ViewBag.CurrentName = searchName;
            ViewBag.CurrentPesel = searchPesel;

            return View(users);
        }


        [HttpGet]
        public IActionResult Dodaj()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Dodaj(DodajUzytkownikaViewModel model)
        {   
            if (!ModelState.IsValid) return View(model); 
    
            if(model.DataUrodzenia.HasValue)
            {
                var birthDateResult = BirthDateValidator.WalidujDateUrodzenia(model.DataUrodzenia.Value);
                if (!birthDateResult.IsValid)
                    ModelState.AddModelError("DataUrodzenia", birthDateResult.Message);
            }

            var phoneResult = PhoneValidator.WalidujNrTelefonu(model.Telefon);
            if (!phoneResult.IsValid) ModelState.AddModelError("Telefon", phoneResult.Message);

            var emailResult = EmailValidator.WalidujEmail(model.Email, _context);
            if (!emailResult.IsValid)
                ModelState.AddModelError("Email", emailResult.ErrorMessage);

            var peselResult = PeselValidator.WalidujPesel(model.Pesel, model.DataUrodzenia, model.Plec, _context);
            if (!peselResult.IsValid)
                ModelState.AddModelError("Pesel", peselResult.ErrorMessage);

            var loginResult = LoginValidator.WalidujLogin(model.Login, _context);
            if (!loginResult.IsValid)
                    ModelState.AddModelError("Login", loginResult.Message);

            if (!ModelState.IsValid) return View(model);

            var user = new Uzytkownik
            {
                Login = model.Login,
                Imie = model.Imie,
                Nazwisko = model.Nazwisko,
                Pesel = model.Pesel,
                DataUrodzenia = model.DataUrodzenia!.Value,
                Plec = model.Plec,
                Email = model.Email,
                Telefon = model.Telefon,
                Miejscowosc = model.Miejscowosc,
                KodPocztowy = model.KodPocztowy,
                Ulica = model.Ulica,
                NumerPosesji = model.NumerPosesji,
                NumerLokalu = model.NumerLokalu
            };

            _context.Uzytkownicy.Add(user);
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Utworzono konto użytkownika ({user.Imie} {user.Nazwisko}).";
            return RedirectToAction("Index");
        }

        public IActionResult Zapomniani(string searchLogin, string searchName, string searchPesel)
        {
            var query = _context.Uzytkownicy
                .Where(u => u.CzyZapomniany == true)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchLogin))
                query = query.Where(u => u.Login.Contains(searchLogin));

            if (!string.IsNullOrEmpty(searchName))
                query = query.Where(u => u.Imie.Contains(searchName) || u.Nazwisko.Contains(searchName));

            if (!string.IsNullOrEmpty(searchPesel))
                query = query.Where(u => u.Pesel.Contains(searchPesel));

            var users = query
                .Select(u => new UzytkownikListItemViewModel
                {
                    Login = u.Login,
                    Imie = u.Imie,
                    Nazwisko = u.Nazwisko,
                    Email = u.Email,
                    Pesel = u.Pesel,
                    DataZapomnienia = u.DataZapomnienia
                })
                .ToList();

            ViewBag.CurrentLogin = searchLogin;
            ViewBag.CurrentName = searchName;
            ViewBag.CurrentPesel = searchPesel;

            return View(users);
        }

        public IActionResult Szczegoly(string login)
        {
            var user = _context.Uzytkownicy.FirstOrDefault(u => u.Login == login);
            if (user == null) return NotFound();

            return View(user);
        }
        [HttpGet]
        public IActionResult Edytuj(string login)
        {
            if (string.IsNullOrEmpty(login)) return BadRequest();

            var user = _context.Uzytkownicy.FirstOrDefault(u => u.Login == login);
            if (user == null) return NotFound();

            var model = new EdytujUzytkownikaViewModel
            {
                Id = user.Id,
                Login = user.Login,
                Imie = user.Imie,
                Nazwisko = user.Nazwisko,
                Pesel = user.Pesel,
                DataUrodzenia = user.DataUrodzenia,
                Plec = user.Plec,
                Email = user.Email,
                Telefon = user.Telefon,
                Miejscowosc = user.Miejscowosc,
                KodPocztowy = user.KodPocztowy,
                Ulica = user.Ulica,
                NumerPosesji = user.NumerPosesji,
                NumerLokalu = user.NumerLokalu
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edytuj(EdytujUzytkownikaViewModel model)
        {
            if (!ModelState.IsValid) return View(model); 
            
            var loginResult = LoginValidator.WalidujLogin(model.Login, _context, model.Id);
            if (!loginResult.IsValid)
                ModelState.AddModelError("Login", loginResult.Message);

            if(model.DataUrodzenia.HasValue)
            {
                var birthDateResult = BirthDateValidator.WalidujDateUrodzenia(model.DataUrodzenia.Value);
                if (!birthDateResult.IsValid)
                    ModelState.AddModelError("DataUrodzenia", birthDateResult.Message);
            }

            var phoneResult = PhoneValidator.WalidujNrTelefonu(model.Telefon);
            if (!phoneResult.IsValid) ModelState.AddModelError("Telefon", phoneResult.Message);

            var emailResult = EmailValidator.WalidujEmail(model.Email, _context, model.Id);
            if (!emailResult.IsValid)
                ModelState.AddModelError("Email", emailResult.ErrorMessage);

            var peselRes = PeselValidator.WalidujPesel(model.Pesel, model.DataUrodzenia, model.Plec, _context, model.Id);
            if (!peselRes.IsValid) 
                ModelState.AddModelError("Pesel", peselRes.ErrorMessage);


            if (!ModelState.IsValid) return View(model);

            var userToUpdate = _context.Uzytkownicy.Find(model.Id);
            if (userToUpdate == null) return NotFound();

            userToUpdate.Login = model.Login;
            userToUpdate.Imie = model.Imie;
            userToUpdate.Nazwisko = model.Nazwisko;
            userToUpdate.Pesel = model.Pesel;
            userToUpdate.DataUrodzenia = model.DataUrodzenia!.Value;
            userToUpdate.Plec = model.Plec;
            userToUpdate.Email = model.Email;
            userToUpdate.Telefon = model.Telefon;
            userToUpdate.Miejscowosc = model.Miejscowosc;
            userToUpdate.KodPocztowy = model.KodPocztowy;
            userToUpdate.Ulica = model.Ulica;
            userToUpdate.NumerPosesji = model.NumerPosesji;
            userToUpdate.NumerLokalu = model.NumerLokalu;

            _context.SaveChanges();
            TempData["SuccessMessage"] = $"Zaktualizowano dane użytkownika ({userToUpdate.Imie} {userToUpdate.Nazwisko}).";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Zapomnij(int id)
        {
            var user = _context.Uzytkownicy
                            .Include(u => u.Uprawnienia)
                            .FirstOrDefault(u => u.Id == id);

            if (user == null) return NotFound();

            user.Uprawnienia.Clear();

            var anon = PeselValidator.GenerujDaneAnonimowe();

            user.Imie = Guid.NewGuid().ToString("N").Substring(0, 8);
            user.Nazwisko = Guid.NewGuid().ToString("N").Substring(0, 10);
            user.Pesel = anon.Pesel;
            user.DataUrodzenia = anon.DataUrodzenia;
            user.Plec = anon.Plec;
            user.ZapomnianyPrzezId = 1;
            
            user.CzyZapomniany = true;
            user.DataZapomnienia = DateTime.Now;

            
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Użytkownik został pomyślnie zapomniany, a jego uprawnienia usunięte.";
            return RedirectToAction("Zapomniani");
        }



    }
}