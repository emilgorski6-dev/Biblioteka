using System.Collections.Generic;

namespace Biblioteka.Web.Models
{
    public class UprawnienieSzczegolyViewModel
    {
        public string NazwaUprawnienia { get; set; } = string.Empty;
        public List<UzytkownikZUprawnieniem> Uzytkownicy { get; set; } = new List<UzytkownikZUprawnieniem>();
    }

    public class UzytkownikZUprawnieniem
    {
        public string Login { get; set; } = string.Empty;
        public string ImieNazwisko { get; set; } = string.Empty;
        public string Rola { get; set; } = string.Empty;
    }
}