using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteka.Web.Data.Entities
{
    [Table("Wypozyczenia_Pozycje")] // Nazwa zgodna z propozycją bazodanowca
    public class WypozyczeniePozycja
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int WypozyczenieId { get; set; }

        [Required]
        public int KsiazkaId { get; set; }

        public DateTime? DataFaktycznegoZwrotu { get; set; }

        // Właściwości nawigacyjne
        [ForeignKey("WypozyczenieId")]
        public virtual Wypozyczenie Wypozyczenie { get; set; } = null!;

        [ForeignKey("KsiazkaId")]
        public virtual Ksiazka Ksiazka { get; set; } = null!;
    }
}