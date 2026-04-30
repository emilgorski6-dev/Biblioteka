using Microsoft.AspNetCore.Mvc;
using Biblioteka.Web.Models;

namespace Biblioteka.Web.Controllers
{
    public class KsiazkiController : Controller
    {
        public static List<KsiazkaListaViewModel> _biblioteka = new List<KsiazkaListaViewModel>
        {
            new KsiazkaListaViewModel { Id = 1, Tytul = "Władca Pierścieni: Drużyna Pierścienia", Autor = "J.R.R. Tolkien", Gatunek = "Fantastyka", RokWydania = 1954, Status = "Dostępna" },
            new KsiazkaListaViewModel { Id = 2, Tytul = "Wiedźmin: Ostatnie życzenie", Autor = "Andrzej Sapkowski", Gatunek = "Fantastyka", RokWydania = 1993, Status = "Wypożyczona" }
        };

        // Widok: /Ksiazki/Lista
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

        // Widok: /Ksiazki/Zarejestruj
        public IActionResult Zarejestruj()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Zarejestruj(KsiazkaListaViewModel model) // Uproszczone na potrzeby przykładu
        {
            if (ModelState.IsValid)
            {
                // 1. Nadajemy ID
                model.Id = _biblioteka.Count > 0 ? _biblioteka.Max(k => k.Id) + 1 : 1;
                
                // 2. Realizujemy wymaganie ZRK-01: Status zawsze "Dostępna"
                model.Status = "Dostępna";

                // 3. Realizujemy wymagania ZRK-02 (dla Managera):
                model.DataRejestracji = DateTime.Now;
                // W przyszłości pobierzesz to z User.Identity.Name
                model.OsobaRejestrujaca = User.Identity?.Name ?? "Nieznany"; 

                // 4. Dodajemy do listy
                _biblioteka.Add(model);

                TempData["SuccessMessage"] = $"Dodano książkę {model.Tytul}.";
                return RedirectToAction("Lista");
            }
            return View(model);
        }

        public IActionResult ZarejestrujZListy(int id)
        {
            var istniejaca = _biblioteka.FirstOrDefault(k => k.Id == id);
            if (istniejaca == null) return NotFound();

            // Tworzymy model z wstępnie wypełnionymi danymi zgodnie z wymaganiem
            var nowyEgzemplarz = new KsiazkaListaViewModel
            {
                Tytul = istniejaca.Tytul,
                Autor = istniejaca.Autor,
                Gatunek = istniejaca.Gatunek
                // Pozostałe pola (Cena, Liczba stron itd.) bibliotekarz wypełni w formularzu
            };

            // Zwracamy ten sam widok "Zarejestruj", ale z wypełnionym modelem
            return View("Zarejestruj", nowyEgzemplarz);
        }

        // Widok: /Ksiazki/Szczegoly/{id}
        public IActionResult Szczegoly(int id)
        {
            var ksiazka = _biblioteka.FirstOrDefault(k => k.Id == id);
            if (ksiazka == null) return NotFound();
            
            return View(ksiazka);
        }
    }

    
}
