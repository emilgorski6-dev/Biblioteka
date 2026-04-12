using System.Linq;
using System.Text.RegularExpressions; // DODAJ TO
using Biblioteka.Web.Data;

namespace Biblioteka.Web.Helpers
{
    public static class EmailValidator
    {
        public const string MsgAlreadyExists = "Adres email został już zarejestrowany dla innego konta.";
        public const string MsgInvalidFormat = "Nieprawidłowy format adresu e-mail"; // DODAJ TO

        public static (bool IsValid, string ErrorMessage) WalidujEmail(string? email, BibliotekaDbContext context, int? userId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return (false, "Email jest wymagany.");

            // STRIKTNY REGEX: Dopuszcza tylko a-z, 0-9, kropki, minusy i plusy.
            // Odrzuci emotikony (TC_94), symbole %% (TC_95) i polskie znaki (TC_96).
            var emailRegex = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (!System.Text.RegularExpressions.Regex.IsMatch(email, emailRegex))
                return (false, "Nieprawidłowy format adresu e-mail");

            // Sprawdzanie unikalności w bazie
            var exists = context.Uzytkownicy.Any(u => u.Email == email && u.Id != userId);

            if (exists)
                return (false, MsgAlreadyExists);

            return (true, string.Empty);
        }
    }
}