using Biblioteka.Web.Data;
using Biblioteka.Web.Data.Entities;
using System.Text.RegularExpressions;
using System.Linq;

namespace Biblioteka.Web.Helpers
{
    public static class PasswordValidator
    {
        public static (bool IsValid, string Message) Waliduj(string password, Uzytkownik user, BibliotekaDbContext context)
        {
            // --- 1. Zasady złożoności (zawsze sprawdzane) ---
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8 || password.Length > 15)
                return (false, "Hasło powinno zawierać od 8 do 15 znaków.");

            var hasUpper = new Regex(@"[A-Z]").IsMatch(password);
            var hasLower = new Regex(@"[a-z]").IsMatch(password);
            var hasDigit = new Regex(@"[0-9]").IsMatch(password);
            var hasSpecial = new Regex(@"[-_!*#$&]").IsMatch(password);
            var onlyAllowedChars = new Regex(@"^[a-zA-Z0-9-_!*#$&]+$").IsMatch(password);

            if (!hasUpper || !hasLower || !hasDigit || !hasSpecial || !onlyAllowedChars)
                return (false, "Hasło powinno zawierać wielką i małą literę, cyfrę oraz znak specjalny -, _, !, *, #, $, &.");

            // --- 2. Sprawdzenie z OBECNYM hasłem (naprawa błędu z NULL) ---
            // Jeśli HasloHash w bazie to "admin123", a admin wpisze "admin123", to wyrzuci błąd.
            // Jeśli w bazie jest NULL, to porównanie (NULL == password) będzie fałszem, więc przejdzie (i słusznie).
            if (user.HasloHash != null && user.HasloHash == password)
            {
                return (false, "Nowe hasło nie może być takie samo jak obecne.");
            }

            // --- 3. Sprawdzenie z HISTORIĄ (3 ostatnie) ---
            var historia = context.HistoriaHasel
                .Where(h => h.UzytkownikId == user.Id && !h.CzyTymczasowe)
                .OrderByDescending(h => h.DataNadania)
                .Take(3)
                .Select(h => h.HasloHash)
                .ToList();

            if (historia.Contains(password))
            {
                return (false, "Hasło nie jest różne od trzech ostatnio nadanych haseł użytkownikowi.");
            }

            return (true, string.Empty);
        }
    }
}