using Microsoft.AspNetCore.Mvc;
using Biblioteka.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Biblioteka.Web.Controllers
{
    public class KsiazkiController : Controller
    {
        public static List<KsiazkaListaViewModel> _biblioteka = new List<KsiazkaListaViewModel>
        {
            new KsiazkaListaViewModel { Id = 1, Tytul = "Władca Pierścieni: Drużyna Pierścienia", Autor = "J.R.R. Tolkien", Gatunek = "Fantastyka", RokWydania = 1954, Status = "Dostępna" },
            new KsiazkaListaViewModel { Id = 2, Tytul = "Wiedźmin: Ostatnie życzenie", Autor = "Andrzej Sapkowski", Gatunek = "Fantastyka", RokWydania = 1993, Status = "Wypożyczona" }
        };

        [HttpGet]
        public IActionResult Lista(string searchString)
        {
            var ksiazki = _biblioteka.AsEnumerable();
            if (!string.IsNullOrEmpty(searchString))
            {
                ksiazki = ksiazki.Where(s => s.Tytul.Contains(searchString, StringComparison.OrdinalIgnoreCase) 
                                          || s.Autor.Contains(searchString, StringComparison.OrdinalIgnoreCase));
                ViewBag.CurrentFilter = searchString;
            }
            return View(ksiazki.ToList());
        }

        [HttpGet]
        public IActionResult Zarejestruj()
        {
            // TEST NA SZTYWNO - tylko to wywoła Toast w rogu
            TempData["SuccessMessage"] = "TEST POŁĄCZENIA - KOMUNIKAT DZIAŁA"; 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Zarejestruj(KsiazkaListaViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Id = _biblioteka.Count > 0 ? _biblioteka.Max(k => k.Id) + 1 : 1;
                model.Status = "Dostępna";
                model.DataRejestracji = DateTime.Now;
                model.OsobaRejestrujaca = User.Identity?.Name ?? "Bibliotekarz Nova"; 
                
                _biblioteka.Add(model);

                TempData["SuccessMessage"] = model.Tytul;
                return RedirectToAction(nameof(Zarejestruj));
            }
            return View(model);
        }

        public IActionResult ZarejestrujZListy(int id)
        {
            var istniejaca = _biblioteka.FirstOrDefault(k => k.Id == id);
            if (istniejaca == null) return NotFound();

            var nowyEgzemplarz = new KsiazkaListaViewModel
            {
                Tytul = istniejaca.Tytul,
                Autor = istniejaca.Autor,
                Gatunek = istniejaca.Gatunek
            };
            return View("Zarejestruj", nowyEgzemplarz);
        }

        public IActionResult Szczegoly(int id)
        {
            var ksiazka = _biblioteka.FirstOrDefault(k => k.Id == id);
            if (ksiazka == null) return NotFound();
            return View(ksiazka);
        }
    }
}