using System.Collections.Generic;
using System.Linq;

namespace Biblioteka.Web.Models
{
    public class KsiazkaFiltrViewModel
    {
        public string? Tytul { get; set; }
        public List<string> WybraniAutorzy { get; set; } = new();
        public List<string> WybraneGatunki { get; set; } = new();   
        public List<string> WybraneWydawnictwa { get; set; } = new();
        public List<string> WybraneStatusy { get; set; } = new();

        // Listy do załadowania dropdownów
        public List<string> DostepniAutorzy { get; set; } = new();
        public List<string> DostepneGatunki { get; set; } = new();
        public List<string> DostepneWydawnictwa { get; set; } = new();
        public List<string> DostepneStatusy { get; set; } = new() { "Dostępna", "Wypożyczona", "W renowacji" };

        public List<KsiazkaListaViewModel> Wyniki { get; set; } = new();
        

        public bool IsFilterActive => 
            !string.IsNullOrEmpty(Tytul) ||
            WybraniAutorzy.Any() ||
            WybraneGatunki.Any() ||
            WybraneWydawnictwa.Any() ||
            WybraneStatusy.Any();
    }
}