using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteka.Web.Data.Entities
{
    [Table("Historia_Hasel")]
    public class HistoriaHasla
    {
        public int Id { get; set; }

        [Column("uzytkownik_id")]
        public int UzytkownikId { get; set; }

        [Column("haslo_hash")]
        public string HasloHash { get; set; }

        [Column("data_nadania")]
        public DateTime DataNadania { get; set; }

        // Właściwość nawigacyjna
        [ForeignKey("UzytkownikId")]
        public virtual Uzytkownik Uzytkownik { get; set; }
    }
}