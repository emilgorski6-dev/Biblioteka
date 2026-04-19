using System.ComponentModel.DataAnnotations;

namespace Biblioteka.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Pole wymagalne.")]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pole wymagalne.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}