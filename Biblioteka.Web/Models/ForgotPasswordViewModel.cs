using System.ComponentModel.DataAnnotations;

namespace Biblioteka.Web.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        public string Login { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}