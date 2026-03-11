using System;
using System.Linq;

namespace Biblioteka.Web.Helpers
{
    public static class PeselValidator
    {
        public static bool CzyPeselJestPoprawny(string pesel, DateTime dataUrodzenia, string plec)
        {
            if (string.IsNullOrWhiteSpace(pesel))
                return false;

            if (pesel.Length != 11)
                return false;

            if (!pesel.All(char.IsDigit))
                return false;

            if (!SprawdzDateUrodzenia(pesel, dataUrodzenia))
                return false;

            if (!SprawdzPlec(pesel, plec))
                return false;

            if (!SprawdzCyfreKontrolna(pesel))
                return false;

            return true;
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

        private static bool SprawdzPlec(string pesel, string plec)
        {
            int cyfraPlecowa = pesel[9] - '0';

            bool peselMezczyzna = cyfraPlecowa % 2 == 1;

            if (plec == "Mężczyzna" && !peselMezczyzna)
                return false;

            if (plec == "Kobieta" && peselMezczyzna)
                return false;

            return true;
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
    }
}
