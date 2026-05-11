namespace Biblioteka.Web.Models
{
    public class WypozyczenieViewModel
    {
        public int Id { get; set; }
        public string Wypozyczajacy { get; set; }
        public string Ksiazka { get; set; }
        public DateTime DataWypozyczenia { get; set; }
        public DateTime DataZwrotu { get; set; }
        public string Status { get; set; }
        public string Bibliotekarz { get; set; }
    }
}