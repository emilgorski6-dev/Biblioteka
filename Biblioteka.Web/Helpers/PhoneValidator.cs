using System.Linq;
using Biblioteka.Web.Data;

namespace Biblioteka.Web.Helpers
{
    public static class PhoneValidator
    {
        public const string MsgAlreadyExists = "Ten numer telefonu jest już przypisany do innego użytkownika.";

        public static (bool IsValid, string Message) WalidujNrTelefonu(string? telefon, BibliotekaDbContext context, int? userId = null)
        {
            if (string.IsNullOrWhiteSpace(telefon))
                return (false, "Numer telefonu jest wymagany.");

            // DODAJ WALIDACJĘ FORMATU (9 cyfr):
            if (telefon.Length != 9 || !telefon.All(char.IsDigit))
                return (false, "Numer telefonu musi zawierać dokładnie 9 cyfr");

            var exists = context.Uzytkownicy.Any(u => u.Telefon == telefon && u.Id != userId);
            if (exists)
                return (false, MsgAlreadyExists);

            return (true, string.Empty);
        }
    }
}