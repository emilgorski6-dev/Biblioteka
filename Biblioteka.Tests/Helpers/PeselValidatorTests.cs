using Xunit;
using Biblioteka.Web.Helpers;
using Biblioteka.Web.Data.Entities;
using System;

namespace Biblioteka.Tests.Helpers
{
    public class PeselValidatorTests
    {
        [Fact]
        public void TC_1_DodanieUzytkownika_DanePoprawne_PowinnoZwrocicSukces()
        {
            string pesel = "99010108970";
            DateTime dataUrodzenia = new DateTime(1999, 1, 1);
            TypPlci plec = TypPlci.Mezczyzna;

            var result = PeselValidator.WalidujAlgorytm(pesel, dataUrodzenia, plec);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void TC_17_WalidacjaPesel_NiezgodnoscZData_PowinnaZwrocicBlad()
        {
            string pesel = "00270345674";
            DateTime dataUrodzenia = new DateTime(2000, 3, 17);
            TypPlci plec = TypPlci.Mezczyzna;

            var result = PeselValidator.WalidujAlgorytm(pesel, dataUrodzenia, plec);

            Assert.False(result.IsValid);
            Assert.Equal("Pierwsze sześć cyfr numeru PESEL nie zgadza się z podaną datą urodzenia", result.ErrorMessage);
        }

        [Fact]
        public void TC_18_WalidacjaPesel_NiezgodnoscPlci_PowinnaZwrocicBlad()
        {
            string pesel = "00270345684";
            DateTime dataUrodzenia = new DateTime(2000, 7, 3);
            TypPlci plec = TypPlci.Mezczyzna;

            var result = PeselValidator.WalidujAlgorytm(pesel, dataUrodzenia, plec);

            Assert.False(result.IsValid);

            var oczekiwanyBlad = "Przedostatnia cyfra numeru PESEL wskazuje na płeć żeńską, a w formularzu wybrano płeć męską.";
            Assert.Equal(oczekiwanyBlad, result.ErrorMessage);
        }
        [Fact]
        public void TC_19_WalidacjaPesel_NiezgodnoscPlciMeskiDlaKobiety_PowinnaZwrocicBlad()
        {
            string pesel = "00270345674";
            DateTime dataUrodzenia = new DateTime(2000, 7, 3);
            TypPlci plec = TypPlci.Kobieta;

            var result = PeselValidator.WalidujAlgorytm(pesel, dataUrodzenia, plec);

            Assert.False(result.IsValid);

            var oczekiwanyBlad = "Przedostatnia cyfra numeru PESEL wskazuje na płeć męską, a w formularzu wybrano płeć żeńską.";
            Assert.Equal(oczekiwanyBlad, result.ErrorMessage);
        }
        [Fact]
        public void TC_20_WalidacjaPesel_BlednaCyfraKontrolna_PowinnaZwrocicBlad()
        {
            string pesel = "00270345675";
            DateTime dataUrodzenia = new DateTime(2000, 7, 3);
            TypPlci plec = TypPlci.Mezczyzna;

            var result = PeselValidator.WalidujAlgorytm(pesel, dataUrodzenia, plec);

            Assert.False(result.IsValid);

            var oczekiwanyBlad = "Błędna cyfra kontrolna PESEL.";
            Assert.Equal(oczekiwanyBlad, result.ErrorMessage);
        }
    }
}