using System.Linq;
using System.Text.RegularExpressions;
using Biblioteka.Web.Data;

namespace Biblioteka.Web.Helpers
{
    public static class EmailValidator
    {
        public const string MsgInvalidFormat = "Błąd składni adresu email. Email powinien mieć format: nazwa_użytkownika@nazwa_domeny_serwera_poczty";
        public const string MsgAlreadyExists = "Adres email został już zarejestrowany dla innego konta.";
        public const string MsgInvalidLength = "Niepoprawna długość adresu email. Adres email powinien zawierać maksymalnie 255 znaków.";
        public const string MsgInvalidAtSymbol = "Nieprawidłowa liczba znaków @. Email musi zawierać dokładnie jeden znak @.";


        public static (bool IsValid, string ErrorMessage) WalidujEmail(string email, BibliotekaDbContext context, int? userId = null)
        {
            if (email.Count(@char => @char == '@') != 1)
                return (false, MsgInvalidAtSymbol);

            if (email.Length > 255)
                return (false, MsgInvalidLength);

            if (!IsValidFormat(email))
                return (false, MsgInvalidFormat);

            if (context.Uzytkownicy.Any(user => user.Email == email && user.Id != userId))
                return (false, MsgAlreadyExists);

            return (true, string.Empty);
        }

        private static bool IsValidFormat(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || email.Contains(" "))
                return false;
            

            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }
    }
}