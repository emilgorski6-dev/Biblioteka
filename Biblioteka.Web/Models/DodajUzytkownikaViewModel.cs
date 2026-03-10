using System.ComponentModel.DataAnnotations;

namespace Biblioteka.Web.Models
{
    public class DodajUzytkownikaViewModel
    {
        [Required]
        public string Login { get; set; }
        
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }
        
        [Required]
        [RegularExpression(@"^\d{9}$")]
        public string Telefon { get; set; }

        [Required]
        public string Imie { get; set; }

        [Required]
        public string Nazwisko { get; set; }

        [Required]
        [StringLength(11)]
        public string Pesel { get; set; }

        [Required]
        public DateTime DataUrodzenia { get; set; }

        [Required]
        public string Plec { get; set; }

        [Required]
        public string Adres { get; set; }

        [Required]
        public string KodPocztowy { get; set; }

        public string NumerLokalu { get; set; }
    }
}