using Biblioteka.Web.Data;
using Biblioteka.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Biblioteka.Web.Services
{
    public class PasswordService
    {
        private readonly BibliotekaDbContext _context;

        // Główny konstruktor dla wstrzykiwania zależności
        public PasswordService(BibliotekaDbContext context)
        {
            _context = context;
        }

        // Konstruktor bezparametrowy (protected) wymagany przez Moq w testach jednostkowych
        protected PasswordService() { }

        // Metoda weryfikująca hasło (Naprawa błędu CS1061 w testach AccountControllerTests)
        public virtual bool VerifyPassword(string password, string hashedPassword)
        {
            // Wersja uproszczona na potrzeby projektu (porównanie tekstowe)
            return password == hashedPassword;
        }

        // Metoda hashująca hasło (uproszczona)
        public virtual string HashPassword(string password)
        {
            return password;
        }

        // Metoda sprawdzająca historię 3 haseł (Wymaganie L-03)
        public virtual async Task<bool> CzyHasloByloUzywane(int uzytkownikId, string noweHaslo)
        {
            var historia = await _context.HistoriaHasel
                .Where(h => h.UzytkownikId == uzytkownikId)
                .OrderByDescending(h => h.DataNadania)
                .Take(3)
                .ToListAsync();

            return historia.Any(h => h.HasloHash == noweHaslo);
        }

        // Metoda generująca hasło automatyczne (Wymaganie L-06)
        public virtual string GenerujHasloAutomatyczne()
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