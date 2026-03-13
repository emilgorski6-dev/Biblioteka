using System.ComponentModel.DataAnnotations;

namespace Biblioteka.Web.Models
{
    public class DodajUzytkownikaViewModel
    {
        [Required] public required string Login { get; set; }
        [Required] public required string Imie { get; set; }
        [Required] public required string Nazwisko { get; set; }
        [Required] public required string Email { get; set; }
        [Required] public required string Telefon { get; set; }
        [Required] public required string Pesel { get; set; }
        [Required] public DateTime DataUrodzenia { get; set; }
        [Required] public required string Plec { get; set; }
        [Required] public required string Miejscowosc { get; set; }
        [Required] public required string KodPocztowy { get; set; }
        [Required] public required string NumerPosesji { get; set; }

        // Pola opcjonalne zostawiamy jako string? (nullable) - brak ostrzeżeń [cite: 48, 50]
        public string? Ulica { get; set; }
        public string? NumerLokalu { get; set; }
    }
}