using System.ComponentModel.DataAnnotations;

namespace Biblioteka.Web.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        public string Login { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}