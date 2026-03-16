using System;
using System.Linq;

namespace Biblioteka.Web.Helpers
{
    public static class PeselValidator
    {
        public static (bool IsValid, string ErrorMessage) WalidujSzczegolowo(string pesel, DateTime dataUrodzenia, string plec)
        {
            if (string.IsNullOrWhiteSpace(pesel) || pesel.Length != 11 || !pesel.All(char.IsDigit))
                return (false, "Niepoprawny format numeru PESEL.");

            if (!SprawdzDateUrodzenia(pesel, dataUrodzenia))
                return (false, "Pierwsze sześć cyfr numeru PESEL nie zgadza się z podaną datą urodzenia");

            int cyfraPlecowa = pesel[9] - '0';
            bool peselMezczyzna = cyfraPlecowa % 2 == 1;
            string p = plec?.ToLower() ?? "";

            if (p == "mężczyzna" && !peselMezczyzna)
                return (false, "Przedostatnia cyfra numeru PESEL wskazuje na płeć żeńską, a w formularzu wybrano płeć męską.");

            if (p == "kobieta" && peselMezczyzna)
                return (false, "Przedostatnia cyfra numeru PESEL wskazuje na płeć męską, a w formularzu wybrano płeć żeńską.");

            if (!SprawdzCyfreKontrolna(pesel))
                return (false, "Nieprawidłowy numer PESEL – niepoprawna cyfra kontrolna.");

            return (true, string.Empty);
        }
        public static bool CzyPeselJestPoprawny(string pesel, DateTime dataUrodzenia, string plec)
        {
            return WalidujSzczegolowo(pesel, dataUrodzenia, plec).IsValid;
        }


        private static bool SprawdzDateUrodzenia(string pesel, DateTime dataUrodzenia)
        {
            int rok = int.Parse(pesel.Substring(0, 2));
            int miesiac = int.Parse(pesel.Substring(2, 2));
            int dzien = int.Parse(pesel.Substring(4, 2));

            int wiek = 1900;

            if (miesiac >= 81 && miesiac <= 92)
            {
                wiek = 1800;
                miesiac -= 80;
            }
            else if (miesiac >= 61 && miesiac <= 72)
            {
                wiek = 2200;
                miesiac -= 60;
            }
            else if (miesiac >= 41 && miesiac <= 52)
            {
                wiek = 2100;
                miesiac -= 40;
            }
            else if (miesiac >= 21 && miesiac <= 32)
            {
                wiek = 2000;
                miesiac -= 20;
            }

            int pelnyRok = wiek + rok;
            DateTime dataZPeselu;

            try
            {
                dataZPeselu = new DateTime(pelnyRok, miesiac, dzien);
            }
            catch
            {
                return false;
            }

            return dataZPeselu.Date == dataUrodzenia.Date;
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


        public static (string Pesel, DateTime DataUrodzenia, string Plec) GenerujDaneAnonimowe()
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

            string reszta = rnd.Next(0, 10000).ToString("D4");
            string p = $"{rr}{mm}{dd}{reszta}";

            int[] wagi = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3 };
            int suma = 0;
            for (int i = 0; i < 10; i++) suma += (p[i] - '0') * wagi[i];
            int cyfraKontrolna = (10 - (suma % 10)) % 10;

            string finalnyPesel = p + cyfraKontrolna;
            string plec = (int.Parse(finalnyPesel[9].ToString()) % 2 == 0) ? "kobieta" : "mężczyzna";

            return (finalnyPesel, data, plec);
        }
    }
}