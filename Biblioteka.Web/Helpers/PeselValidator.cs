using System;
using System.Linq;
using Biblioteka.Web.Data.Entities;

namespace Biblioteka.Web.Helpers
{
    public static class PeselValidator
    {
        // --- Stałe z komunikatami błędów ---
        public const string MsgInvalidFormat = "Nieprawidłowy format numeru PESEL";
        public const string MsgDateMismatch = "Pierwsze sześć cyfr numeru PESEL nie zgadza się z podaną datą urodzenia";
        public const string MsgInvalidChecksum = "Błędna cyfra kontrolna PESEL.";

        // Rozdzielone komunikaty dla płci
        public const string MsgGenderMismatchMale = "Przedostatnia cyfra numeru PESEL wskazuje na płeć żeńską, a w formularzu wybrano płeć męską.";
        public const string MsgGenderMismatchFemale = "Przedostatnia cyfra numeru PESEL wskazuje na płeć męską, a w formularzu wybrano płeć żeńską.";

        // Komunikat unikalności (do użycia w kontrolerze)
        public const string MsgPeselExists = "Podany numer PESEL jest już przypisany do innego użytkownika w systemie.";

        /// <summary>
        /// Główna metoda walidująca algorytm PESEL.
        /// </summary>
        public static (bool IsValid, string ErrorMessage) WalidujAlgorytm(string pesel, DateTime? dataUrodzenia, TypPlci plec)
        {
            if (string.IsNullOrWhiteSpace(pesel) || pesel.Length != 11 || !pesel.All(char.IsDigit))
                return (false, MsgInvalidFormat);

            if (!SprawdzDateUrodzenia(pesel, dataUrodzenia))
                return (false, MsgDateMismatch);

            // Walidacja płci z konkretnym komunikatem
            var plecResult = SprawdzPlec(pesel, plec);
            if (!plecResult.IsValid)
                return (false, plecResult.Message);

            if (!SprawdzCyfreKontrolna(pesel))
                return (false, MsgInvalidChecksum);

            return (true, string.Empty);
        }

        private static (bool IsValid, string Message) SprawdzPlec(string pesel, TypPlci plec)
        {
            int cyfraPlecowa = pesel[9] - '0';
            bool jestMezczyznaW_Pesel = cyfraPlecowa % 2 == 1;

            if (plec == TypPlci.Mezczyzna && !jestMezczyznaW_Pesel)
            {
                return (false, MsgGenderMismatchMale);
            }
            
            if (plec == TypPlci.Kobieta && jestMezczyznaW_Pesel)
            {
                return (false, MsgGenderMismatchFemale);
            }

            return (true, string.Empty);
        }

        private static bool SprawdzDateUrodzenia(string pesel, DateTime? dataUrodzenia)
        {
            if (!dataUrodzenia.HasValue) return false;

            int rok = (pesel[0] - '0') * 10 + (pesel[1] - '0');
            int miesiac = (pesel[2] - '0') * 10 + (pesel[3] - '0');
            int dzien = (pesel[4] - '0') * 10 + (pesel[5] - '0');

            int wiek = 1900;
            if (miesiac > 80) { wiek = 1800; miesiac -= 80; }
            else if (miesiac > 20) { wiek = 2000; miesiac -= 20; }
            else if (miesiac > 40) { wiek = 4000; miesiac -= 40; } 
            else if (miesiac > 60) { wiek = 2200; miesiac -= 60; }

            try
            {
                DateTime dataZPeselu = new DateTime(wiek + rok, miesiac, dzien);
                return dataZPeselu.Date == dataUrodzenia.Value.Date;
            }
            catch { return false; }
        }

        private static bool SprawdzCyfreKontrolna(string pesel)
        {
            int[] wagi = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
            int suma = Enumerable.Range(0, 10).Sum(i => (pesel[i] - '0') * wagi[i]);
            int cyfraKontrolna = (10 - (suma % 10)) % 10;
            return cyfraKontrolna == (pesel[10] - '0');
        }

        public static (string Pesel, DateTime DataUrodzenia, TypPlci Plec) GenerujDaneAnonimowe()
        {
            Random rnd = new Random();
            DateTime start = new DateTime(1950, 1, 1);
            DateTime data = start.AddDays(rnd.Next((new DateTime(2005, 1, 1) - start).Days));

            int m = data.Month + (data.Year >= 2000 ? 20 : 0);
            int losowe = rnd.Next(1000, 9999);
            string p = $"{data:yy}{m:00}{data:dd}{losowe:0000}";

            int[] wagi = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
            int suma = Enumerable.Range(0, 10).Sum(i => (p[i] - '0') * wagi[i]);
            string finalnyPesel = p + ((10 - (suma % 10)) % 10);

            TypPlci plec = (losowe % 10 % 2 == 0) ? TypPlci.Kobieta : TypPlci.Mezczyzna;
            return (finalnyPesel, data, plec);
        }
    }
}