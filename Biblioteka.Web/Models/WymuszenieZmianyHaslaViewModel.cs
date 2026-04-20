using System.ComponentModel.DataAnnotations;

namespace Biblioteka.Web.Models
{
    public class WymuszenieZmianyHaslaViewModel
    {
        public int Id { get; set; }
        
        public string? Login { get; set; }

        [Required(ErrorMessage = "Pole wymagalne.")]
        // Tutaj nie dodajemy walidacji Regex, bo zrobi to Twój PasswordValidator w kontrolerze
        public string? NoweHaslo { get; set; }

        [Required(ErrorMessage = "Pole wymagalne.")]
        [Compare("NoweHaslo", ErrorMessage = "Wprowadzone hasła różnią się od siebie.")]
        public string? PotwierdzHaslo { get; set; }
    }
}