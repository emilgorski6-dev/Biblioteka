using System.Linq;
using System.Text.RegularExpressions;

namespace Biblioteka.Web.Helpers
{
    public static class EmailValidator
    {
        public static bool IsValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            if (email.Length > 255) return false;
            if (email.Count(c => c == '@') != 1) return false;

            if (email.Contains(" ")) return false;

            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }
    }
}