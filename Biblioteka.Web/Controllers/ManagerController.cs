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
            // Manager widzi te same dane, ale z naciskiem na datę i osobę
            // Narazie używamy tej samej statycznej listy co w KsiazkiController
            var historia = KsiazkiController._biblioteka; 
            
            if (!historia.Any())
            {
                // Dokumentacja: Brak zarejestrowanych książek
                ViewBag.EmptyMessage = "Nie zarejestrowano jeszcze żadnych książek";
            }
            
            return View(historia);
        }
    }
}