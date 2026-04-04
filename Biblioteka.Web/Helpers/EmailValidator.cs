using System.Linq;
using Biblioteka.Web.Data;

namespace Biblioteka.Web.Helpers
{
    public static class EmailValidator
    {
        public const string MsgAlreadyExists = "Adres email został już zarejestrowany dla innego konta.";

        public static (bool IsValid, string ErrorMessage) WalidujEmail(string email, BibliotekaDbContext context, int? userId = null)
        {
            // Nie sprawdzamy już formatu ani długości - to zrobił ModelState na podstawie ViewModelu.
            // Skupiamy się wyłącznie na logice bazy danych.

            var exists = context.Uzytkownicy.Any(u => u.Email == email && u.Id != userId);

            if (exists)
                return (false, MsgAlreadyExists);

            return (true, string.Empty);
        }
    }
}