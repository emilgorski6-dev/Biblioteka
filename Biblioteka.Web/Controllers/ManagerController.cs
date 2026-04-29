using Microsoft.AspNetCore.Mvc;

namespace Biblioteka.Web.Controllers
{
    public class ManagerController : Controller
    {
        // Widok główny: /Manager/Index
        public IActionResult Index()
        {
            return View();
        }

        // Widok listy: /Manager/Rejestracje
        public IActionResult Rejestracje()
        {
            return View();
        }
    }
}