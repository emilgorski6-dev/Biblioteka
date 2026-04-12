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
            // Arrange - Dane Tomasza Nowego (TC_1)
            string pesel = "99010108970";
            DateTime dataUrodzenia = new DateTime(1999, 1, 1);
            TypPlci plec = TypPlci.Mezczyzna;

            // Act
            var result = PeselValidator.WalidujAlgorytm(pesel, dataUrodzenia, plec);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void TC_17_WalidacjaPesel_NiezgodnoscZData_PowinnaZwrocicBlad()
        {
            // Arrange - Dane z Twojej dokumentacji (Bożydarowy PESEL, ale błędna data)
            // PESEL 002703... to 03.07.2000
            string pesel = "00270345674";
            DateTime dataUrodzenia = new DateTime(2000, 3, 17); // Błędna data wg dokumentacji
            TypPlci plec = TypPlci.Mezczyzna;

            // Act
            var result = PeselValidator.WalidujAlgorytm(pesel, dataUrodzenia, plec);

            // Assert - Sprawdzamy, czy komunikat zgadza się z logiką systemu
            Assert.False(result.IsValid);
            Assert.Equal("Pierwsze sześć cyfr numeru PESEL nie zgadza się z podaną datą urodzenia", result.ErrorMessage);
        }

        [Fact]
        public void TC_18_WalidacjaPesel_NiezgodnoscPlci_PowinnaZwrocicBlad()
        {
            string pesel = "00270345684";
            DateTime dataUrodzenia = new DateTime(2000, 7, 3);
            TypPlci plec = TypPlci.Mezczyzna; // W formularzu wybrano Mężczyzna -> błąd!

            // Act
            var result = PeselValidator.WalidujAlgorytm(pesel, dataUrodzenia, plec);

            // Assert
            Assert.False(result.IsValid);

            // Zwróć uwagę na treść komunikatu - teraz to PESEL wskazuje na płeć żeńską
            var oczekiwanyBlad = "Przedostatnia cyfra numeru PESEL wskazuje na płeć żeńską, a w formularzu wybrano płeć męską.";
            Assert.Equal(oczekiwanyBlad, result.ErrorMessage);
        }
        [Fact]
        public void TC_19_WalidacjaPesel_NiezgodnoscPlciMeskiDlaKobiety_PowinnaZwrocicBlad()
        {
            string pesel = "00270345674";
            DateTime dataUrodzenia = new DateTime(2000, 7, 3);
            TypPlci plec = TypPlci.Kobieta; // W formularzu wybrano Kobieta -> błąd!

            // Act
            var result = PeselValidator.WalidujAlgorytm(pesel, dataUrodzenia, plec);

            // Assert
            Assert.False(result.IsValid);

            // Komunikat musi być precyzyjny: PESEL mówi "Mężczyzna", formularz mówi "Kobieta"
            var oczekiwanyBlad = "Przedostatnia cyfra numeru PESEL wskazuje na płeć męską, a w formularzu wybrano płeć żeńską.";
            Assert.Equal(oczekiwanyBlad, result.ErrorMessage);
        }
        [Fact]
        public void TC_20_WalidacjaPesel_BlednaCyfraKontrolna_PowinnaZwrocicBlad()
        {
            // Arrange - Dane z Twojej dokumentacji
            // PESEL: 00270345675 
            // Matematycznie: dla 0027034567 cyfra kontrolna POWINNA wynosić 4.
            // Podanie 5 na końcu to celowy błąd matematyczny.
            string pesel = "00270345675";
            DateTime dataUrodzenia = new DateTime(2000, 7, 3);
            TypPlci plec = TypPlci.Mezczyzna;

            // Act
            var result = PeselValidator.WalidujAlgorytm(pesel, dataUrodzenia, plec);

            // Assert
            Assert.False(result.IsValid);

            // Upewnij się, że komunikat jest spójny z tym, co zwraca Twój PeselValidator
            var oczekiwanyBlad = "Błędna cyfra kontrolna PESEL.";
            Assert.Equal(oczekiwanyBlad, result.ErrorMessage);
        }
    }
}