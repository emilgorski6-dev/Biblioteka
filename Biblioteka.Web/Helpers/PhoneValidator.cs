using System.Linq;
using Biblioteka.Web.Data;

namespace Biblioteka.Web.Helpers
{
    public static class PhoneValidator
    {
        public const string MsgAlreadyExists = "Ten numer telefonu jest już przypisany do innego użytkownika.";

        public static (bool IsValid, string Message) WalidujNrTelefonu(string telefon, BibliotekaDbContext context, int? userId = null)
        {
            // Format (9 cyfr) sprawdza już atrybut [RegularExpression] w ViewModelu.
            var exists = context.Uzytkownicy.Any(u => u.Telefon == telefon && u.Id != userId);

            if (exists)
                return (false, MsgAlreadyExists);

            return (true, string.Empty);
        }
    }
}