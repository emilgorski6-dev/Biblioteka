using System.ComponentModel.DataAnnotations;

namespace Biblioteka.Web.Models
{
    public class UzytkownikListItemViewModel
    {
        public required string Login { get; set; }
        public required string Imie { get; set; }
        public required string Nazwisko { get; set; }
        public required string Email { get; set; }

        // To jest brakujące ogniwo:
        public required string Pesel { get; set; }
    }
}