using Microsoft.AspNetCore.Mvc;

namespace Biblioteka.Web.Controllers
{
    public class UzytkownicyController : Controller
    {
        // Wyświetla stronę panelu admina (Views/Uzytkownicy/Index.cshtml)
        public IActionResult Index()
        {
            return View();
        }
    }
}