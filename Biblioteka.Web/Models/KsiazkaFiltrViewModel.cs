using System.Collections.Generic;

namespace Biblioteka.Web.Models
{
    public class KsiazkaFiltrViewModel
    {
        // Wybrane wartości (pojedyncze stringi dla dropdownów)
        public string? Tytul { get; set; }
        public string? WybranyAutor { get; set; }
        public string? WybranyGatunek { get; set; }
        public string? WybraneWydawnictwo { get; set; }
        public string? WybranyStatus { get; set; }

        // Listy do załadowania dropdownów
        public List<string> DostepniAutorzy { get; set; } = new();
        public List<string> DostepneGatunki { get; set; } = new();
        public List<string> DostepneWydawnictwa { get; set; } = new();
        public List<string> DostepneStatusy { get; set; } = new() { "Dostępna", "Wypożyczona", "W renowacji" };

        public List<KsiazkaListaViewModel> Wyniki { get; set; } = new();

        public bool IsFilterActive => !string.IsNullOrEmpty(Tytul) || 
                                     !string.IsNullOrEmpty(WybranyAutor) || 
                                     !string.IsNullOrEmpty(WybranyGatunek) || 
                                     !string.IsNullOrEmpty(WybraneWydawnictwo) || 
                                     !string.IsNullOrEmpty(WybranyStatus);
    }
}