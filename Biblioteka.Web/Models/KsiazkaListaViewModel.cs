using System.ComponentModel.DataAnnotations;

namespace Biblioteka.Web.Models
{
    public class KsiazkaListaViewModel
    {
        public int Id { get; set; }

        [Display(Name = "tytuł książki")]
        [Required(ErrorMessage = "Tytuł książki jest wymagana")]
        public string Tytul { get; set; } = string.Empty;

        [Display(Name = "autorzy książki")]
        [Required(ErrorMessage = "Autorzy książki jest wymagana")]
        public string Autor { get; set; } = string.Empty;

        [Display(Name = "gatunek")]
        [Required(ErrorMessage = "Gatunek jest wymagana")]
        public string Gatunek { get; set; } = string.Empty;

        [Display(Name = "liczba stron")]
        [Required(ErrorMessage = "Liczba stron jest wymagana")]
        [Range(1, int.MaxValue, ErrorMessage = "Wartość w polu liczba stron musi być większa od zera")]
        public int? LiczbaStron { get; set; }

        [Display(Name = "rok wydania")]
        [Required(ErrorMessage = "Rok wydania jest wymagana")]
        [Range(1, 3000, ErrorMessage = "Wprowadź poprawny rok wydania")]
        public int? RokWydania { get; set; }

        [Display(Name = "cena książki")]
        [Required(ErrorMessage = "Cena książki jest wymagana")]
        [Range(0.01, 999999.99, ErrorMessage = "Cena musi być większa od zera")]
        public decimal? Cena { get; set; }

        [Display(Name = "liczba rejestrowanych sztuk")]
        [Required(ErrorMessage = "Liczba rejestrowanych sztuk jest wymagana")]
        [Range(1, 1000, ErrorMessage = "Wartość w polu liczba rejestrowanych sztuk musi być większa od zera")]
        public int? LiczbaSztuk { get; set; }

        [Display(Name = "wydawnictwo")]
        [Required(ErrorMessage = "Wydawnictwo jest wymagana")]
        public string Wydawnictwo { get; set; } = string.Empty;

        [Display(Name = "opis książki")]
        [Required(ErrorMessage = "Opis książki jest wymagana")]
        public string Opis { get; set; } = string.Empty;

        public string Status { get; set; } = "Dostępna";

        // --- POLA POTRZEBNE DLA MANAGERCONTROLLER ---
        public DateTime DataRejestracji { get; set; } = DateTime.Now;
        public string OsobaRejestrujaca { get; set; } = string.Empty;
    }
}