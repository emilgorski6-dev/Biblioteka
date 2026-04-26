using Biblioteka.Web.Data;
using Biblioteka.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Biblioteka.Web.Services
{
    public class PasswordService
    {
        private readonly BibliotekaDbContext _context;

        public PasswordService(BibliotekaDbContext context)
        {
            _context = context;
        }

        // Metoda sprawdzająca historię 3 haseł (Wymaganie L-03)
        public async Task<bool> CzyHasloByloUzywane(int uzytkownikId, string noweHaslo)
        {
            var historia = await _context.HistoriaHasel
                .Where(h => h.UzytkownikId == uzytkownikId)
                .OrderByDescending(h => h.DataNadania)
                .Take(3)
                .ToListAsync();

            // W produkcji tutaj porównywalibyśmy hashe haseł
            return historia.Any(h => h.HasloHash == noweHaslo);
        }

        // Metoda generująca hasło automatyczne (Wymaganie L-06)
        // 10 znaków: 3 wielkie, 3 małe, 2 cyfry, 2 specjalne
        public string GenerujHasloAutomatyczne()
        {
            const string wielkie = "ABCDEFGHIJKLMNPQRSTUVWXYZ";
            const string male = "abcdefghijkmnopqrstuvwxyz";
            const string cyfry = "123456789";
            const string specjalne = "-_!*#$&";

            var random = new Random();
            var password = new StringBuilder();

            for (int i = 0; i < 3; i++) password.Append(wielkie[random.Next(wielkie.Length)]);
            for (int i = 0; i < 3; i++) password.Append(male[random.Next(male.Length)]);
            for (int i = 0; i < 2; i++) password.Append(cyfry[random.Next(cyfry.Length)]);
            for (int i = 0; i < 2; i++) password.Append(specjalne[random.Next(specjalne.Length)]);

            // Przemieszanie znaków
            return new string(password.ToString().ToCharArray().OrderBy(s => random.Next()).ToArray());
        }
    }
}