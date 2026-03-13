using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteka.Web.Data.Entities
{
    [Table("Uprawnienia")]
    public class Uprawnienie
    {
        public int Id { get; set; }

        public string Nazwa { get; set; }

        public string? Opis { get; set; }

        // Relacja wiele-do-wielu z Uzytkownikami
        public virtual ICollection<Uzytkownik> Uzytkownicy { get; set; } = new List<Uzytkownik>();
    }
}