using System.ComponentModel.DataAnnotations;
namespace Biblioteka.Web.Models
{
    public class KsiazkaListaViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Tytuł książki jest wymagany")]
        public string Tytul { get; set; } = null!;

        [Required(ErrorMessage = "Autorzy książki są wymagani")]
        public string Autor { get; set; } = null!;

        [Required(ErrorMessage = "Gatunek jest wymagany")]
        public string Gatunek { get; set; } = null!;

        [Required(ErrorMessage = "Wydawnictwo jest wymagane")]
        public string Wydawnictwo { get; set; } = null!;

        [Required(ErrorMessage = "Rok wydania jest wymagany")]
        [Range(1000, 2026, ErrorMessage = "Rok wydania nie może być z przyszłości")] // Walidacja z dok.
        public int RokWydania { get; set; }

        [Required(ErrorMessage = "Liczba stron jest wymagana")]
        [Range(1, int.MaxValue, ErrorMessage = "Wartość w polu Liczba stron musi być większa od zera")]
        public int LiczbaStron { get; set; }

        [Required(ErrorMessage = "Cena jest wymagana")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Wartość w polu Cena musi być większa od zera")]
        public decimal Cena { get; set; }

        [Required(ErrorMessage = "Liczba sztuk jest wymagana")]
        [Range(1, 1000, ErrorMessage = "Wartość w polu Liczba rejestrowanych sztuk musi być większa od zera")]
        public int LiczbaSztuk { get; set; }

        public string Opis { get; set; } = string.Empty;
        public string Status { get; set; } = "Dostępna";
        
        // Dane dla Managera (ZRK-02)
        public DateTime DataRejestracji { get; set; }
        public string OsobaRejestrujaca { get; set; } = string.Empty;
    }
}