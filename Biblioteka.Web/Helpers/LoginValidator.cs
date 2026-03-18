using System.Linq;
using Biblioteka.Web.Data;

namespace Biblioteka.Web.Helpers
{
    public static class LoginValidator
    {
        public const string MsgAlreadyExists = "Podany login jest już zajęty przez innego użytkownika.";

        public static (bool IsValid, string Message) WalidujLogin(string login, BibliotekaDbContext context, int? userId = null)
        {
            if (context.Uzytkownicy.Any(user => user.Login == login && user.Id != userId))
                return (false, MsgAlreadyExists);

            return (true, string.Empty);
        }
    }
}