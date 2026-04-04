namespace Biblioteka.Web.Models
{
    public class ZapiszUprawnieniaModel
    {
        public required string Login { get; set; }
        public required List<string> WybraneRole { get; set; }
    }
}