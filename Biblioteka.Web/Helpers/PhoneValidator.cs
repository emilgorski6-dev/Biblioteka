using System.Linq;
using Biblioteka.Web.Data;

namespace Biblioteka.Web.Helpers
{
    public static class PhoneValidator
    {
        public const string MsgInvalidFormat = "Numer telefonu musi składać się z dokładnie 9 cyfr.";
        public const string MsgAlreadyExists = "Ten numer telefonu jest już przypisany do innego użytkownika.";

        public static (bool IsValid, string Message) WalidujNrTelefonu(string telefon, BibliotekaDbContext context, int? userId = null)
        {
            // 1. Sprawdzenie formatu
            if (string.IsNullOrWhiteSpace(telefon) || telefon.Length != 9 || !telefon.All(char.IsDigit))
                return (false, MsgInvalidFormat);

            // 2. NOWE: Sprawdzenie unikalności w bazie (z uwzględnieniem edycji)
            var exists = context.Uzytkownicy.Any(u => u.Telefon == telefon && u.Id != userId);
            if (exists)
                return (false, MsgAlreadyExists);

            return (true, string.Empty);
        }
    }
}