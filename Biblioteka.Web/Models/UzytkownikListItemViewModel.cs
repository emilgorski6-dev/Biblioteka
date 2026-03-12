using System.ComponentModel.DataAnnotations;

namespace Biblioteka.Web.Models
{
    public class UzytkownikListItemViewModel
    {
        public string Login { get; set; }

        public string Imie { get; set; }

        public string Nazwisko { get; set; }

        public string Email { get; set; }
    }
}