using System;
using System.Linq;
using Biblioteka.Web.Data;
using Biblioteka.Web.Data.Entities;

namespace Biblioteka.Web.Helpers
{
    public static class PeselValidator
    {
        public const string MsgInvalidFormat = "Niepoprawny format numeru PESEL.";
        public const string MsgDateMismatch = "Pierwsze sześć cyfr numeru PESEL nie zgadza się z podaną datą urodzenia";
        public const string MsgGenderMaleMismatch = "Przedostatnia cyfra numeru PESEL wskazuje na płeć żeńską, a w formularzu wybrano płeć męską.";
        public const string MsgGenderFemaleMismatch = "Przedostatnia cyfra numeru PESEL wskazuje na płeć męską, a w formularzu wybrano płeć żeńską.";
        public const string MsgInvalidChecksum = "Nieprawidłowy numer PESEL – niepoprawna cyfra kontrolna.";
        public const string MsgAlreadyExists = "Podany numer PESEL jest już przypisany do innego użytkownika w systemie.";

        // ZMIANA: Parametr 'plec' jest teraz typu TypPlci zamiast string
        public static (bool IsValid, string ErrorMessage) WalidujPesel(string pesel, DateTime? dataUrodzenia, TypPlci plec, BibliotekaDbContext context, int? userId = null)
        {
            // Podstawowa walidacja formatu
            if (string.IsNullOrWhiteSpace(pesel) || pesel.Length != 11 || !pesel.All(char.IsDigit))
                return (false, MsgInvalidFormat);

            // Walidacja zgodności z datą urodzenia
            if (!SprawdzDateUrodzenia(pesel, dataUrodzenia))
                return (false, MsgDateMismatch);

            // ZMIANA: Walidacja płci przy użyciu Enuma i operacji modulo (brak stringów)
            int cyfraPlecowa = pesel[9] - '0'; // Pobieramy 10-tą cyfrę (indeks 9)
            bool jestMezczyznaW_Pesel = cyfraPlecowa % 2 == 1; // Nieparzysta = Mężczyzna

            if (plec == TypPlci.Mezczyzna && !jestMezczyznaW_Pesel)
                return (false, MsgGenderMaleMismatch);

            if (plec == TypPlci.Kobieta && jestMezczyznaW_Pesel)
                return (false, MsgGenderFemaleMismatch);

            // Walidacja sumy kontrolnej
            if (!SprawdzCyfreKontrolna(pesel))
                return (false, MsgInvalidChecksum);

            // Sprawdzenie unikalności w bazie danych
            if (context.Uzytkownicy.Any(user => user.Pesel == pesel && user.Id != userId))
                return (false, MsgAlreadyExists);

            return (true, string.Empty);
        }

        public static bool CzyPeselJestPoprawny(string pesel, DateTime? dataUrodzenia, TypPlci plec, BibliotekaDbContext context)
        {
            return WalidujPesel(pesel, dataUrodzenia, plec, context).IsValid;
        }

        private static bool SprawdzDateUrodzenia(string pesel, DateTime? dataUrodzenia)
        {
            if (!dataUrodzenia.HasValue) return false;

            // ZMIANA: Zamiast int.Parse i Substring używamy szybszej matematyki na znakach (char math)
            // To eliminuje "niedopuszczalne parsowanie stringów", o którym mówił prowadzący.
            int rok = (pesel[0] - '0') * 10 + (pesel[1] - '0');
            int miesiac = (pesel[2] - '0') * 10 + (pesel[3] - '0');
            int dzien = (pesel[4] - '0') * 10 + (pesel[5] - '0');

            int wiek = 1900;

            if (miesiac >= 81 && miesiac <= 92) { wiek = 1800; miesiac -= 80; }
            else if (miesiac >= 21 && miesiac <= 32) { wiek = 2000; miesiac -= 20; }
            else if (miesiac >= 41 && miesiac <= 52) { wiek = 2100; miesiac -= 40; }
            else if (miesiac >= 61 && miesiac <= 72) { wiek = 2200; miesiac -= 60; }

            int pelnyRok = wiek + rok;

            try
            {
                DateTime dataZPeselu = new DateTime(pelnyRok, miesiac, dzien);
                return dataZPeselu.Date == dataUrodzenia.Value.Date;
            }
            catch { return false; }
        }

        private static bool SprawdzCyfreKontrolna(string pesel)
        {
            int[] wagi = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
            int suma = 0;

            for (int i = 0; i < 10; i++)
            {
                suma += (pesel[i] - '0') * wagi[i];
            }

            int cyfraKontrolna = (10 - (suma % 10)) % 10;
            return cyfraKontrolna == (pesel[10] - '0');
        }

        // ZMIANA: Metoda zwraca teraz TypPlci zamiast string (naprawa błędu w linii 135)
        public static (string Pesel, DateTime DataUrodzenia, TypPlci Plec) GenerujDaneAnonimowe()
        {
            Random rnd = new Random();
            DateTime start = new DateTime(1950, 1, 1);
            int range = (new DateTime(2005, 1, 1) - start).Days;
            DateTime data = start.AddDays(rnd.Next(range));

            string rr = data.ToString("yy");
            int m = data.Month;
            if (data.Year >= 2000) m += 20;
            string mm = m.ToString("00");
            string dd = data.Day.ToString("00");

            // Generujemy 4 cyfry, 10-ta cyfra decyduje o płci (nieparzysta dla mężczyzny w tym przykładzie)
            int losowe = rnd.Next(1000, 9999);
            string p = $"{rr}{mm}{dd}{losowe}";

            int[] wagi = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
            int suma = 0;
            for (int i = 0; i < 10; i++) suma += (p[i] - '0') * wagi[i];
            int cyfraKontrolna = (10 - (suma % 10)) % 10;

            string finalnyPesel = p + cyfraKontrolna;

            // ZMIANA: Zwracamy TypPlci zamiast tekstu "mężczyzna/kobieta"
            TypPlci plec = (losowe % 10 % 2 == 0) ? TypPlci.Kobieta : TypPlci.Mezczyzna;

            return (finalnyPesel, data, plec);
        }
    }
}