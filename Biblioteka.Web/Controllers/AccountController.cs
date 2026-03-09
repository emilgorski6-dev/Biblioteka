using Microsoft.AspNetCore.Mvc;

namespace Biblioteka.Web.Controllers
{
    public class AccountController : Controller
    {
        // Wyświetla formularz logowania (Views/Account/Login.cshtml)
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Odbiera dane z formularza logowania po kliknięciu "Zaloguj się"
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // Tutaj w przyszłości sprawdzisz hasło. Na razie po prostu przekierowujemy na stronę główną.
            return RedirectToAction("Index", "Home");
        }

        // Wyświetla formularz rejestracji (Views/Account/Register.cshtml)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Wyświetla formularz przypomnienia hasła (Views/Account/ForgotPassword.cshtml)
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
    }
}