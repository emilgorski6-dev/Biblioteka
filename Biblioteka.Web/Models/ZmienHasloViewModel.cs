using System.ComponentModel.DataAnnotations;

namespace Biblioteka.Web.Models
{
    public class ZmienHasloViewModel
    {
        public int Id { get; set; }
        public string? Login { get; set; }
        public string? PelnaNazwa { get; set; }

        [Required(ErrorMessage = "Pole hasło jest wymagane.")]
        [DataType(DataType.Password)]
        public string? NoweHaslo { get; set; }
    }
}