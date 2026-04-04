using Microsoft.AspNetCore.Mvc;
using Biblioteka.Web.Models; // Upewnij się, że masz LoginViewModel

namespace Biblioteka.Web.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken] // KLUCZOWE: Zabezpieczenie przed atakami
        public IActionResult Login(LoginViewModel model) // Używamy ViewModelu zamiast surowych stringów
        {
            if (!ModelState.IsValid) return View(model);

            // Tutaj w przyszłości trafi logika weryfikacji hasła
            return RedirectToAction("Dashboard", "Uzytkownicy");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // Prowadzący doceni, że Logout jest przez POST (bezpieczniej)
            return RedirectToAction("Index", "Home");
        }
    }
}