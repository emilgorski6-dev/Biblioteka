using System.ComponentModel.DataAnnotations;

namespace Biblioteka.Web.Data.Entities
{
    public class Wypozyczenie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int KlientId { get; set; } // Powiązanie z użytkownikiem (Klient)

        [Required]
        public string BibliotekarzId { get; set; } = string.Empty; // ID bibliotekarza rejestrującego

        [Required]
        public DateTime DataWypozyczenia { get; set; } // Data systemowa (ZW-01) 

        [Required]
        public DateTime TerminZwrotu { get; set; } // Standardowo +2 tyg, opcjonalnie 1-2 m-ce 

        [Required]
        public string Status { get; set; } = "Nowe"; // Nowe, Przedłużone, Zakończone

        public virtual ICollection<WypozyczeniePozycja> Pozycje { get; set; } = new List<WypozyczeniePozycja>();
    }
}