namespace Biblioteka.Web.Models
{
    public class UprawnienieItemViewModel
    {
        public int Id { get; set; }
        public string Nazwa { get; set; } = string.Empty;
        public string Opis { get; set; } = string.Empty;
        public int LiczbaUzytkownikow { get; set; } // Tu trafi realna liczba z bazy
        public string BadgeClass => Nazwa.ToUpper() switch
        {
            "ADMINISTRATOR" => "badge-all",    // Pomarańczowy
            "BIBLIOTEKARZ"  => "badge-insert", // Zielony
            "MANAGER"       => "badge-alter",  // Fioletowy
            "KLIENT"        => "badge-select", // Niebieski
            _               => "badge-default" // Szary (dla innych)
        };
    };
}