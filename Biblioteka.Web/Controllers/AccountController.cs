using Microsoft.AspNetCore.Mvc;

namespace Biblioteka.Web.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login (Logika po kliknięciu "Zaloguj się")
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // Tutaj w przyszłości dodasz weryfikację z bazą danych
            // Jeśli poprawne, przekieruj do Dashboardu:
            return RedirectToAction("Dashboard", "Uzytkownicy");
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View(); // Ten plik masz w folderze Ksiiazki/Zarejestruj lub Account
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            // Logika czyszczenia sesji
            return RedirectToAction("Index", "Home");
        }
    }
}