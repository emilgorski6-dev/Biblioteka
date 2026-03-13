using System.Linq;
using System.Text.RegularExpressions;

namespace Biblioteka.Web.Helpers
{
    public static class EmailValidator
    {
        public static bool IsValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            // Kryterium: Max 255 znaków
            if (email.Length > 255) return false;

            // Kryterium: Dokładnie jeden znak @
            if (email.Count(c => c == '@') != 1) return false;

            // Kryterium: Składnia nazwa@domena.tld
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }
    }
}