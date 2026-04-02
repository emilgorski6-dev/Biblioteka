using System.Collections.Generic;

namespace Biblioteka.Web.Models
{
    public class UprawnienieSzczegolyViewModel
    {
        public string NazwaUprawnienia { get; set; }
        public List<UzytkownikZUprawnieniem> Uzytkownicy { get; set; } = new List<UzytkownikZUprawnieniem>();
    }

    public class UzytkownikZUprawnieniem
    {
        public string Login { get; set; }
        public string ImieNazwisko { get; set; }
        public string Rola { get; set; }
    }
}