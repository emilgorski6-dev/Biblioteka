using System.ComponentModel.DataAnnotations;

namespace Biblioteka.Web.Data.Entities
{
    public class Ksiazka
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Tytul { get; set; } = string.Empty;

        [Required]
        public string Autorzy { get; set; } = string.Empty;

        public string Gatunek { get; set; } = string.Empty;

        public string Wydawnictwo { get; set; } = string.Empty;

        public int RokWydania { get; set; }

        public int LiczbaStron { get; set; }

        public decimal Cena { get; set; }

        public int LiczbaSztuk { get; set; }

        public string Opis { get; set; } = string.Empty;

        public string Status { get; set; } = "Dostępna";

        // --- DODAJ TE POLA TERAZ ---
        public DateTime DataRejestracji { get; set; } = DateTime.Now;
        public string OsobaRejestrujaca { get; set; } = string.Empty;
    }
}