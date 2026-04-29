using Microsoft.AspNetCore.Mvc;

namespace Biblioteka.Web.Controllers
{
    public class KsiazkiController : Controller
    {
        // Widok: /Ksiazki/Lista
        public IActionResult Lista()
        {
            return View();
        }

        // Widok: /Ksiazki/Szczegoly/{id}
        public IActionResult Szczegoly(int id)
        {
            return View();
        }

        // Widok: /Ksiazki/Zarejestruj
        public IActionResult Zarejestruj()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Zarejestruj(string tytul) // Uproszczone na potrzeby przykładu
        {
         // Logika zapisu do bazy...
        TempData["SuccessMessage"] = $"Dodano książkę {tytul}";
        return RedirectToAction("Lista");
        }
    }

    
}
