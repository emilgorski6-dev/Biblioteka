namespace Biblioteka.Web.Models
{
    public class UprawnienieItemViewModel
    {
        public string Id { get; set; }
        public string Nazwa { get; set; }
        public string Opis { get; set; }
        public string BadgeClass { get; set; }
        public int LiczbaUzytkownikow { get; set; } // Tu trafi realna liczba z bazy
    }
}