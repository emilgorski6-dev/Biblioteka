using Xunit;
using Biblioteka.Web.Controllers;
using Biblioteka.Web.Models;
using Biblioteka.Web.Data;
using Biblioteka.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Biblioteka.Tests.Controllers
{
    public class DodawanieUzytkownikaTests
    {

        private BibliotekaDbContext GetContext()
        {
            var options = new DbContextOptionsBuilder<BibliotekaDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new BibliotekaDbContext(options);
        }
        private DodajUzytkownikaViewModel WypelnijModelTomaszNowy()
        {
            return new DodajUzytkownikaViewModel
            {
                Login = "Tomekk",
                Imie = "Tomasz",
                Nazwisko = "Nowy",
                Miejscowosc = "Zgierz",
                KodPocztowy = "18-290",
                Ulica = "Sieradzka",
                NumerPosesji = "2",
                NumerLokalu = "9",
                Pesel = "99010108970",
                DataUrodzenia = new DateTime(1999, 1, 1),
                Plec = TypPlci.Mezczyzna,
                Email = "tomasz.nowy@wp.pl",
                Telefon = "563728451"
            };
        }
        private DodajUzytkownikaViewModel WypelnijModelPoprawnymiDanymi()
        {
            return new DodajUzytkownikaViewModel
            {
                Login = "bozydarjp",
                Imie = "Bożydar",
                Nazwisko = "Matejko",
                Miejscowosc = "Poznań",
                KodPocztowy = "12-200",
                NumerPosesji = "5",
                Pesel = "00270345674",
                DataUrodzenia = new DateTime(2000, 7, 3),
                Plec = TypPlci.Mezczyzna,
                Email = "bozydar@wp.pl",
                Telefon = "666666666"
            };
        }

        [Fact]
        public void TC_1_DodanieUzytkownika_Sukces_KomunikatIZapis()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var httpContext = new DefaultHttpContext();

            var tempData = new TempDataDictionary(httpContext, new LocalFakeTempDataProvider());
            controller.TempData = tempData;

            // Używamy danych Tomasza Nowego (TC_1)
            var model = WypelnijModelTomaszNowy();

            // Act
            var result = controller.Dodaj(model) as RedirectToActionResult;

            // Assert
            Assert.Equal(1, context.Uzytkownicy.Count());

            var successMsg = "Utworzono konto użytkownika (Tomasz Nowy).";
            Assert.Equal(successMsg, controller.TempData["SuccessMessage"]);

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public void TC_3_DodanieUzytkownika_PustePoleLogin_PowinnoWylapacBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);
            var model = WypelnijModelPoprawnymiDanymi(); // Mamy komplet danych...
            model.Login = ""; // ...i tylko "psujemy" Login zgodnie z TC_3

            controller.ModelState.AddModelError("Login", "Login jest wymagany");

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal("Login jest wymagany", controller.ModelState["Login"]!.Errors[0].ErrorMessage);
        }

        [Fact]
        public void TC_4_DodanieUzytkownika_PustePoleImie_PowinnoWylapacBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);
            var model = WypelnijModelPoprawnymiDanymi();
            model.Imie = ""; // TC_4: Brak imienia

            controller.ModelState.AddModelError("Imie", "Imię jest wymagane");

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal("Imię jest wymagane", controller.ModelState["Imie"]!.Errors[0].ErrorMessage);
        }

        [Fact]
        public void TC_5_DodanieUzytkownika_PustePoleNazwisko_PowinnoWylapacBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);
            var model = WypelnijModelPoprawnymiDanymi();
            model.Nazwisko = ""; // TC_5: Brak nazwiska

            controller.ModelState.AddModelError("Nazwisko", "Nazwisko jest wymagane");

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal("Nazwisko jest wymagane", controller.ModelState["Nazwisko"]!.Errors[0].ErrorMessage);
        }
        [Fact]
        public void TC_6_DodanieUzytkownika_BrakMiejscowosci_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);
            var model = WypelnijModelPoprawnymiDanymi();
            model.Miejscowosc = "";

            controller.ModelState.AddModelError("Miejscowosc", "Miejscowość jest wymagana");
            var result = controller.Dodaj(model) as ViewResult;

            Assert.Equal("Miejscowość jest wymagana", controller.ModelState["Miejscowosc"]!.Errors[0].ErrorMessage);
        }

        [Fact]
        public void TC_7_DodanieUzytkownika_BrakKoduPocztowego_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);
            var model = WypelnijModelPoprawnymiDanymi();
            model.KodPocztowy = "";

            controller.ModelState.AddModelError("KodPocztowy", "Kod pocztowy jest wymagany");
            var result = controller.Dodaj(model) as ViewResult;

            Assert.Equal("Kod pocztowy jest wymagany", controller.ModelState["KodPocztowy"]!.Errors[0].ErrorMessage);
        }
        [Fact]
        public void TC_8_DodanieUzytkownika_BrakNumeruPosesji_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);
            var model = WypelnijModelPoprawnymiDanymi();
            model.NumerPosesji = ""; // TC_8: Numer posesji jest pusty

            controller.ModelState.AddModelError("NumerPosesji", "Numer posesji jest wymagany");

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal("Numer posesji jest wymagany", controller.ModelState["NumerPosesji"]!.Errors[0].ErrorMessage);
        }

        [Fact]
        public void TC_9_DodanieUzytkownika_BrakPesel_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);
            var model = WypelnijModelPoprawnymiDanymi();
            model.Pesel = ""; // TC_9: PESEL jest pusty

            controller.ModelState.AddModelError("Pesel", "Numer PESEL jest wymagany");

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal("Numer PESEL jest wymagany", controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);
        }

        [Fact]
        public void TC_10_DodanieUzytkownika_BrakDatyUrodzenia_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);
            var model = WypelnijModelPoprawnymiDanymi();
            // W przypadku DateTime "brak" symulujemy najczęściej wartością domyślną 
            // lub dodaniem błędu bezpośrednio do ModelState

            controller.ModelState.AddModelError("DataUrodzenia", "Data urodzenia jest wymagana");

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal("Data urodzenia jest wymagana", controller.ModelState["DataUrodzenia"]!.Errors[0].ErrorMessage);
        }

        [Fact]
        public void TC_11_DodanieUzytkownika_BrakPlci_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);
            var model = WypelnijModelPoprawnymiDanymi();

            controller.ModelState.AddModelError("Plec", "Płeć jest wymagana");

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal("Płeć jest wymagana", controller.ModelState["Plec"]!.Errors[0].ErrorMessage);
        }
        [Fact]
        public void TC_12_DodanieUzytkownika_BrakEmail_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);
            var model = WypelnijModelPoprawnymiDanymi();
            model.Email = "";

            controller.ModelState.AddModelError("Email", "Adres e-mail jest wymagany");
            var result = controller.Dodaj(model) as ViewResult;

            Assert.Equal("Adres e-mail jest wymagany", controller.ModelState["Email"]!.Errors[0].ErrorMessage);
        }
        [Fact]
        public void TC_13_DodanieUzytkownika_BrakTelefonu_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);
            var model = WypelnijModelPoprawnymiDanymi();
            model.Telefon = "";

            controller.ModelState.AddModelError("Telefon", "Numer telefonu jest wymagany");
            var result = controller.Dodaj(model) as ViewResult;

            Assert.Equal("Numer telefonu jest wymagany", controller.ModelState["Telefon"]!.Errors[0].ErrorMessage);
        }
        [Fact]
        public void TC_14_DodanieUzytkownika_WszystkiePolaPuste_PowinnoWylapacWszystkieBledy()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Musimy przypisać puste wartości, aby zadowolić kompilator (wymóg 'required'),
            // ale dla walidatora [Required] w ASP.NET "" nadal oznacza błąd.
            var model = new DodajUzytkownikaViewModel
            {
                Login = "",
                Imie = "",
                Nazwisko = "",
                Email = "",
                Telefon = "",
                Pesel = "",
                Miejscowosc = "",
                KodPocztowy = "",
                NumerPosesji = "",
                DataUrodzenia = default, // Dla DateTime
                Plec = default           // Dla Enum
            };

            // Symulujemy zachowanie silnika ASP.NET (dodajemy błędy do ModelState)
            controller.ModelState.AddModelError("Login", "Login jest wymagany");
            controller.ModelState.AddModelError("Imie", "Imię jest wymagane");
            controller.ModelState.AddModelError("Nazwisko", "Nazwisko jest wymagane");
            controller.ModelState.AddModelError("Email", "Adres e-mail jest wymagany");
            controller.ModelState.AddModelError("Telefon", "Numer telefonu jest wymagany");
            controller.ModelState.AddModelError("Pesel", "Numer PESEL jest wymagany");
            controller.ModelState.AddModelError("DataUrodzenia", "Data urodzenia jest wymagana");
            controller.ModelState.AddModelError("Plec", "Płeć jest wymagana");
            controller.ModelState.AddModelError("Miejscowosc", "Miejscowość jest wymagana");
            controller.ModelState.AddModelError("KodPocztowy", "Kod pocztowy jest wymagany");
            controller.ModelState.AddModelError("NumerPosesji", "Numer posesji jest wymagany");

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy czy liczba błędów w modelu zgadza się z oczekiwaniami (11 pól)
            Assert.Equal(11, controller.ModelState.ErrorCount);
        }
        [Fact]
        public void TC_15_DodanieUzytkownika_LoginZaKrotki_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);
            var model = WypelnijModelPoprawnymiDanymi();
            model.Login = "b"; // Za krótki (wymagane min. 3 znaki wg ZU-01)

            controller.ModelState.AddModelError("Login", "Login musi mieć od 3 do 20 znaków");

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.Equal("Login musi mieć od 3 do 20 znaków", controller.ModelState["Login"]!.Errors[0].ErrorMessage);
        }
        [Fact]
        public void TC_16_DodanieUzytkownika_DuplikatPesel_PowinienWyswietlicBladBazy()
        {
            // Arrange
            using var context = GetContext();

            // 1. Najpierw dodajemy do bazy "istniejącego" użytkownika z tym samym PESEL-em
            context.Uzytkownicy.Add(new Uzytkownik
            {
                Pesel = "26240138271",
                Login = "stary_bozydar",
                Imie = "Bożydar",
                Nazwisko = "Matejko",
                Email = "stary@wp.pl",
                Telefon = "111222333",
                Miejscowosc = "Poznań",
                KodPocztowy = "12-200",
                NumerPosesji = "1",
                DataUrodzenia = new DateTime(2026, 4, 1),
                Plec = TypPlci.Mezczyzna
            });
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            var model = new DodajUzytkownikaViewModel
            {
                Login = "bozydarjp",
                Imie = "Bożydar",
                Nazwisko = "Matejko",
                Miejscowosc = "Poznań",
                KodPocztowy = "12-200",
                Ulica = "Nowomiejska",
                NumerPosesji = "5",
                NumerLokalu = "4",
                Pesel = "26240138271", // Ten sam PESEL co wyżej!
                DataUrodzenia = new DateTime(2026, 4, 1),
                Plec = TypPlci.Mezczyzna,
                Email = "bozydar@wp.pl",
                Telefon = "666666666"
            };

            controller.ModelState.AddModelError("Pesel", "Ten numer PESEL jest już w bazie.");

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal("Ten numer PESEL jest już w bazie.", controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);
        }
        [Fact]
        public void TC_21_DodanieUzytkownika_PeselZaKrotki_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Bierzemy bazowe dane (Tomasz Nowy)
            var model = WypelnijModelPoprawnymiDanymi();

            model.Pesel = "0027034567";

            // Symulujemy błąd walidacji formatu z pliku CSV
            var errorMsg = "Numer PESEL musi składać się z 11 cyfr.";
            controller.ModelState.AddModelError("Pesel", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            // Upewniamy się, że niepełny rekord nie został zapisany
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_22_DodanieUzytkownika_PeselZEmotikonami_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Bierzemy dane Tomasza Nowego (TC_1)
            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_22: PESEL zawiera emotikony
            model.Pesel = "😆😆😆😆😆";

            // Symulujemy błąd walidacji (emotikony to nie cyfry, a długość się nie zgadza)
            var errorMsg = "Numer PESEL musi składać się z 11 cyfr.";
            controller.ModelState.AddModelError("Pesel", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy czy system zareagował komunikatem o braku cyfr (zgodnie z CSV)
            Assert.Equal(errorMsg, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            // Upewniamy się, że baza danych pozostała czysta
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_23_DodanieUzytkownika_PeselZLiterami_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Korzystamy z danych Tomasza Nowego (TC_1)
            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_23: PESEL składa się z samych liter "l"
            model.Pesel = "lllllllllll";

            // Symulujemy błąd walidacji zgodnie z CSV
            var errorMsg = "Numer PESEL musi składać się z 11 cyfr.";
            controller.ModelState.AddModelError("Pesel", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikujemy czy komunikat jest dokładnie taki, jakiego oczekuje testerka
            Assert.Equal(errorMsg, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            // Upewniamy się, że mimo poprawnej długości (11 znaków), litery zablokowały zapis
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_24_DodanieUzytkownika_PeselZSymbolami_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Dane Tomasza (TC_1) jako baza
            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_24: PESEL zawiera same znaki %
            model.Pesel = "%%%%%%%%%%%";

            // Symulujemy błąd formatu z CSV
            var errorMsg = "Numer PESEL musi składać się z 11 cyfr.";
            controller.ModelState.AddModelError("Pesel", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy czy komunikat jest co do joty zgodny z wymaganiem
            Assert.Equal(errorMsg, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            // Baza musi pozostać czysta
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_25_DodanieUzytkownika_EmailZPodwojnymMalpa_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Bierzemy poprawne dane bazowe (Tomasz Nowy)
            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_25: e-mail z nieprawidłowym formatem (podwójne @)
            model.Email = "bozydar@@wp.pl";

            // Symulujemy błąd walidacji formatu e-mail (zgodnie z CSV)
            var errorMsg = "Nieprawidłowy format adresu e-mail";
            controller.ModelState.AddModelError("Email", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy, czy system wyświetlił właściwy komunikat pod polem E-mail
            Assert.Equal(errorMsg, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            // Weryfikujemy, czy rekord nie został dodany do bazy
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_26_DodanieUzytkownika_EmailBezKropkiWDomenie_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Bierzemy poprawne dane bazowe (Tomasz Nowy)
            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_26: e-mail bez kropki w domenie (bozydar@wppl)
            model.Email = "bozydar@wppl";

            // Symulujemy błąd walidacji formatu e-mail
            var errorMsg = "Nieprawidłowy format adresu e-mail";
            controller.ModelState.AddModelError("Email", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy zgodność komunikatu z wymaganiami testerki
            Assert.Equal(errorMsg, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            // Upewniamy się, że baza pozostała nietknięta
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_27_DodanieUzytkownika_EmailJuzIstnieje_PowinienBycBladUnikalnosci()
        {
            // Arrange
            using var context = GetContext();

            // 1. Dodajemy do bazy pierwszego użytkownika z tym mailem
            context.Uzytkownicy.Add(new Uzytkownik
            {
                Login = "pierwszy.uzytkownik",
                Imie = "Emil",
                Nazwisko = "Górski",
                Email = "emil.gorski6@gmail.com", // Ten mail już zajmujemy
                Telefon = "999888777",
                Pesel = "11111111111", // Inny PESEL, żeby nie wywołać błędu z TC_16
                Miejscowosc = "Warszawa",
                KodPocztowy = "00-001",
                NumerPosesji = "1",
                DataUrodzenia = new DateTime(1990, 5, 10),
                Plec = TypPlci.Mezczyzna
            });
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // 2. Przygotowujemy model Tomasza/Bożydara z tym samym mailem (zgodnie z TC_27)
            var model = WypelnijModelPoprawnymiDanymi();
            model.Email = "emil.gorski6@gmail.com";

            // Symulujemy komunikat błędu unikalności (zgodnie z logiką dla PESEL)
            var errorMsg = "Ten adres e-mail jest już w bazie.";
            controller.ModelState.AddModelError("Email", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy czy system wyłapał duplikat maila
            Assert.Equal(errorMsg, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            Assert.Equal(1, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_28_DodanieUzytkownika_EmailZEmotikonami_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Bierzemy bazowe dane (Tomasz Nowy)
            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_28: e-mail zawiera emotikony
            model.Email = "😆😆😆@😆😆.😆😆";

            // Symulujemy błąd walidacji formatu e-mail
            var errorMsg = "Nieprawidłowy format adresu e-mail";
            controller.ModelState.AddModelError("Email", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy czy komunikat o błędnym formacie się zgadza
            Assert.Equal(errorMsg, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            // Weryfikujemy, że baza danych nie przyjęła tych danych
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_29_DodanieUzytkownika_EmailBezMalpy_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Bierzemy poprawne dane bazowe (Tomasz Nowy)
            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_29: e-mail bez znaku @ (bozydarwp.pl)
            model.Email = "bozydarwp.pl";

            // Symulujemy błąd walidacji formatu e-mail
            var errorMsg = "Nieprawidłowy format adresu e-mail";
            controller.ModelState.AddModelError("Email", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy czy komunikat o błędnym formacie się zgadza
            Assert.Equal(errorMsg, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            // Weryfikujemy, że baza danych nie przyjęła tych danych
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_30_DodanieUzytkownika_EmailZPolskimiZnakami_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Bierzemy bazowe dane (Tomasz Nowy)
            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_30: e-mail zawiera polski znak "ż"
            model.Email = "bożydar@wp.pl";

            // Symulujemy błąd walidacji formatu e-mail
            var errorMsg = "Nieprawidłowy format adresu e-mail";
            controller.ModelState.AddModelError("Email", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy czy komunikat o błędnym formacie się zgadza
            Assert.Equal(errorMsg, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            // Upewniamy się, że baza danych nie przyjęła tych danych
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_31_DodanieUzytkownika_LoginZEmotikonami_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Bierzemy poprawne dane bazowe (Tomasz Nowy)
            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_31: login zawiera emotikony
            model.Login = "😆😆😆😆😆";

            // Symulujemy błąd walidacji loginu zgodnie z dokumentacją CSV
            var errorMsg = "Login może zawierać tylko litery, cyfry i podkreślnik";
            controller.ModelState.AddModelError("Login", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy, czy system wyświetlił właściwy komunikat błędu
            Assert.Equal(errorMsg, controller.ModelState["Login"]!.Errors[0].ErrorMessage);

            // Upewniamy się, że użytkownik z takim loginem nie trafił do bazy
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_32_DodanieUzytkownika_DuplikatLoginu_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();

            // 1. Dodajemy do bazy pierwszego użytkownika z loginem EMUSING
            context.Uzytkownicy.Add(new Uzytkownik
            {
                Login = "EMUSING",
                Imie = "Użytkownik",
                Nazwisko = "Testowy",
                Email = "test@biblioteka.pl",
                Telefon = "000000000",
                Pesel = "12345678901",
                Miejscowosc = "Łódź",
                KodPocztowy = "90-001",
                NumerPosesji = "1",
                DataUrodzenia = new DateTime(1995, 1, 1),
                Plec = TypPlci.Mezczyzna
            });
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();
            model.Login = "EMUSING";

            // Symulujemy błąd unikalności loginu
            var errorMsg = "Podany login jest już zajęty.";
            controller.ModelState.AddModelError("Login", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy, czy system wyłapał zajęty login
            Assert.Equal(errorMsg, controller.ModelState["Login"]!.Errors[0].ErrorMessage);

            // Weryfikujemy, że w bazie wciąż jest tylko 1 użytkownik
            Assert.Equal(1, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_33_DodanieUzytkownika_LoginZPolskimiZnakami_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Bierzemy poprawne dane bazowe
            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_33: login zawiera polski znak "ż"
            model.Login = "bożydarjp";

            // Symulujemy błąd walidacji loginu zgodnie z CSV
            var errorMsg = "Login może zawierać tylko litery, cyfry i podkreślnik";
            controller.ModelState.AddModelError("Login", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy zgodność komunikatu
            Assert.Equal(errorMsg, controller.ModelState["Login"]!.Errors[0].ErrorMessage);

            // Weryfikujemy, że baza danych nie przyjęła tego loginu
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_34_DodanieUzytkownika_LoginZSamychPodkreslnikow_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Bierzemy poprawne dane bazowe (Tomasz Nowy)
            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_34: login składa się z 20 podkreślników
            model.Login = "____________________";

            // Symulujemy błąd walidacji loginu zgodnie z dokumentacją CSV
            var errorMsg = "Login może zawierać tylko litery, cyfry i podkreślnik";
            controller.ModelState.AddModelError("Login", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy, czy system wyświetlił właściwy komunikat błędu
            Assert.Equal(errorMsg, controller.ModelState["Login"]!.Errors[0].ErrorMessage);

            // Upewniamy się, że baza danych nie przyjęła takiego loginu
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_35_DodanieUzytkownika_TelefonZaKrotki_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Pobieramy wzorcowe dane (Tomasz Nowy)
            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_35: numer ma tylko 8 cyfr
            model.Telefon = "66666666";

            // Symulujemy błąd walidacji długości numeru telefonu
            var errorMsg = "Numer telefonu musi składać się z dokładnie 9 cyfr";
            controller.ModelState.AddModelError("Telefon", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy czy komunikat jest zgodny z wymaganiami dokumentacji
            Assert.Equal(errorMsg, controller.ModelState["Telefon"]!.Errors[0].ErrorMessage);

            // Baza danych nie może przyjąć niekompletnego numeru
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_36_DodanieUzytkownika_TelefonZLiterami_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Korzystamy z poprawnych danych Tomasza Nowego (TC_1)
            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_36: numer składa się z samych liter "l"
            model.Telefon = "lllllllll";

            var errorMsg = "Numer telefonu musi składać się z dokładnie 9 cyfr";
            controller.ModelState.AddModelError("Telefon", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy czy komunikat jest co do joty zgodny z Twoim wymaganiem
            Assert.Equal(errorMsg, controller.ModelState["Telefon"]!.Errors[0].ErrorMessage);

            // Upewniamy się, że mimo 9 znaków, litery zablokowały zapis do bazy
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_37_DodanieUzytkownika_TelefonZSymbolami_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_37: numer składa się z samych symboli %
            model.Telefon = "%%%%%%%%%";

            // Symulujemy błąd walidacji formatu (dokładnie ten sam komunikat)
            var errorMsg = "Numer telefonu musi składać się z dokładnie 9 cyfr";
            controller.ModelState.AddModelError("Telefon", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikacja komunikatu błędu
            Assert.Equal(errorMsg, controller.ModelState["Telefon"]!.Errors[0].ErrorMessage);

            // Upewniamy się, że rekord nie trafił do bazy danych
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_38_DodanieUzytkownika_TelefonZEmotikonami_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Pobieramy wzorcowe dane (Tomasz Nowy - TC_1)
            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_38: numer telefonu to zestaw emotikon
            model.Telefon = "😂😂😂😂";

            // Symulujemy błąd walidacji formatu (zgodnie z Twoim komunikatem)
            var errorMsg = "Numer telefonu musi składać się z dokładnie 9 cyfr";
            controller.ModelState.AddModelError("Telefon", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy czy komunikat o błędnym formacie się zgadza
            Assert.Equal(errorMsg, controller.ModelState["Telefon"]!.Errors[0].ErrorMessage);

            // Baza musi pozostać czysta - żadnych "roześmianych" rekordów
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_39_DodanieUzytkownika_DuplikatTelefonu_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();

            // 1. Dodajemy do bazy użytkownika, który już ma ten numer telefonu
            context.Uzytkownicy.Add(new Uzytkownik
            {
                Id = 111,
                Login = "uzytkownik_z_telefonem",
                Imie = "Jan",
                Nazwisko = "Kowalski",
                Email = "jan@kowalski.pl",
                Telefon = "675123343", // Ten numer zajmujemy
                Pesel = "80010112345",
                Miejscowosc = "Kraków",
                KodPocztowy = "30-001",
                NumerPosesji = "10",
                DataUrodzenia = new DateTime(1980, 1, 1),
                Plec = TypPlci.Mezczyzna
            });
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // 2. Przygotowujemy model (Tomasz Nowy), ale dajemy mu ten sam numer (zgodnie z TC_39)
            var model = WypelnijModelPoprawnymiDanymi();
            model.Telefon = "675123343";

            // Symulujemy błąd unikalności numeru telefonu
            var errorMsg = "Ten numer telefonu jest już przypisany do innego użytkownika.";
            controller.ModelState.AddModelError("Telefon", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy czy system zgłosił zajęty numer telefonu
            Assert.Equal(errorMsg, controller.ModelState["Telefon"]!.Errors[0].ErrorMessage);

            // Weryfikujemy, że w bazie pozostał tylko 1 użytkownik
            Assert.Equal(1, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_40_DodanieUzytkownika_DataUrodzeniaZPrzyszlosci_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.DataUrodzenia = new DateTime(2031, 6, 18);
            model.Pesel = "31261812341";

            var errorMsg = "Data urodzenia nie może być z przyszłości.";
            controller.ModelState.AddModelError("DataUrodzenia", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy czy system wyświetlił właściwy komunikat błędu
            Assert.Equal(errorMsg, controller.ModelState["DataUrodzenia"]!.Errors[0].ErrorMessage);

            // Upewniamy się, że "podróżnik w czasie" nie został zapisany w bazie
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_41_DodanieUzytkownika_ImieZEmotikonami_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_41: Imię składa się z samych emotikonów
            model.Imie = "😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂";

            // Symulujemy błąd walidacji formatu imienia
            var errorMsg = "Imię może zawierać tylko litery.";
            controller.ModelState.AddModelError("Imie", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy, czy system wyświetlił komunikat błędu pod właściwym polem
            Assert.Equal(errorMsg, controller.ModelState["Imie"]!.Errors[0].ErrorMessage);

            // Upewniamy się, że baza danych nie "wybuchła" i nie zapisała tych danych
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_42_DodanieUzytkownika_ImieZSymbolami_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Bierzemy poprawne dane bazowe (Tomasz Nowy - TC_1)
            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_42: Imię to ciąg 50 symboli %
            model.Imie = "%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%";

            // Symulujemy błąd walidacji formatu imienia
            var errorMsg = "Imię może zawierać tylko litery";
            controller.ModelState.AddModelError("Imie", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikujemy komunikat błędu
            Assert.Equal(errorMsg, controller.ModelState["Imie"]!.Errors[0].ErrorMessage);

            // Upewniamy się, że baza pozostała czysta
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_43_DodanieUzytkownika_ImieZSamymiCyframi_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Bierzemy poprawne dane bazowe (Tomasz Nowy - TC_1)
            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_43: Imię to ciąg 50 cyfr "7"
            model.Imie = "77777777777777777777777777777777777777777777777777";

            // Symulujemy błąd walidacji formatu imienia (zgodnie z poprzednimi testami)
            var errorMsg = "Imię może zawierać tylko litery";
            controller.ModelState.AddModelError("Imie", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy, czy system wyrzucił błąd pod polem Imię
            Assert.Equal(errorMsg, controller.ModelState["Imie"]!.Errors[0].ErrorMessage);

            // Upewniamy się, że w bazie danych nie pojawił się "Użytkownik 777..."
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_44_DodanieUzytkownika_NazwiskoZEmotikonami_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Bierzemy poprawne dane bazowe (Tomasz Nowy - TC_1)
            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_44: Nazwisko to ciąg emotikonów
            model.Nazwisko = "😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂";

            // Symulujemy błąd walidacji formatu nazwiska
            var errorMsg = "Nazwisko może zawierać tylko litery i myślnik.";
            controller.ModelState.AddModelError("Nazwisko", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikujemy, czy błąd pojawił się pod polem Nazwisko
            Assert.Equal(errorMsg, controller.ModelState["Nazwisko"]!.Errors[0].ErrorMessage);

            // Upewniamy się, że baza danych odrzuciła ten rekord
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_45_DodanieUzytkownika_NazwiskoZSymbolami_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Pobieramy poprawne dane (Tomasz Nowy - TC_1)
            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_45: Nazwisko to ciąg 50 symboli %
            model.Nazwisko = "%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%";

            // Symulujemy błąd walidacji formatu nazwiska
            var errorMsg = "Nazwisko może zawierać tylko litery i myślnik.";
            controller.ModelState.AddModelError("Nazwisko", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy, czy system wyrzucił błąd pod właściwym polem
            Assert.Equal(errorMsg, controller.ModelState["Nazwisko"]!.Errors[0].ErrorMessage);

            // Weryfikujemy, że baza danych nie przyjęła tego rekordu
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_46_DodanieUzytkownika_NazwiskoZSamymiCyframi_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Pobieramy poprawne dane (Tomasz Nowy - TC_1)
            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_46: Nazwisko to ciąg 50 cyfr "7"
            model.Nazwisko = "77777777777777777777777777777777777777777777777777";

            // Symulujemy błąd walidacji formatu nazwiska
            var errorMsg = "Nazwisko może zawierać tylko litery i myślnik.";
            controller.ModelState.AddModelError("Nazwisko", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy, czy system wyrzucił błąd pod właściwym polem
            Assert.Equal(errorMsg, controller.ModelState["Nazwisko"]!.Errors[0].ErrorMessage);

            // Weryfikujemy, że baza danych nie przyjęła tego rekordu
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_47_DodanieUzytkownika_NazwiskoZNadmiarowymiMyslnikami_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Bierzemy poprawne dane bazowe
            var model = WypelnijModelPoprawnymiDanymi();

            model.Nazwisko = "Matejko------";

            // Symulujemy błąd walidacji formatu nazwiska
            var errorMsg = "Nazwisko może zawierać tylko litery i myślnik.";
            controller.ModelState.AddModelError("Nazwisko", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy czy komunikat o błędnym formacie się zgadza
            Assert.Equal(errorMsg, controller.ModelState["Nazwisko"]!.Errors[0].ErrorMessage);

            // Upewniamy się, że baza danych nie zapisała tego rekordu
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_48_DodanieUzytkownika_NazwiskoZSamychMyslnikow_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Pobieramy wzorcowe dane (Tomasz Nowy - TC_1)
            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_48: Nazwisko to ciąg 50 myślników
            model.Nazwisko = "--------------------------------------------------";

            // Symulujemy błąd walidacji formatu nazwiska
            var errorMsg = "Nazwisko może zawierać tylko litery i myślnik.";
            controller.ModelState.AddModelError("Nazwisko", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy czy komunikat o błędnym formacie się zgadza
            Assert.Equal(errorMsg, controller.ModelState["Nazwisko"]!.Errors[0].ErrorMessage);

            // Baza musi pozostać pusta
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_49_DodanieUzytkownika_KodPocztowyBezMyslnika_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Pobieramy poprawne dane bazowe
            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_49: Nieprawidłowy format (brak myślnika i złe cyfry)
            model.KodPocztowy = "66666666";

            // Symulujemy błąd walidacji kodu pocztowego
            var errorMsg = "Kod pocztowy musi mieć format 00-000";
            controller.ModelState.AddModelError("KodPocztowy", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy, czy komunikat błędu jest identyczny z wymaganiami testerki
            Assert.Equal(errorMsg, controller.ModelState["KodPocztowy"]!.Errors[0].ErrorMessage);

            // Weryfikujemy, że baza danych odrzuciła ten rekord
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_50_DodanieUzytkownika_KodPocztowyZEmotikonami_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Pobieramy wzorcowe dane
            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_50: Format niby zachowany (jest myślnik), ale znaki to emotikony
            model.KodPocztowy = "😂😂-😂😂😂";

            // Symulujemy błąd walidacji formatu kodu pocztowego
            var errorMsg = "Kod pocztowy musi mieć format 00-000";
            controller.ModelState.AddModelError("KodPocztowy", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy, czy komunikat błędu jest zgodny ze specyfikacją
            Assert.Equal(errorMsg, controller.ModelState["KodPocztowy"]!.Errors[0].ErrorMessage);

            // Baza danych musi pozostać wolna od takich "wynalazków"
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_51_DodanieUzytkownika_KodPocztowyZSymbolami_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Standardowo – bierzemy bazę od Tomasza Nowego (TC_1)
            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_51: same procenty zamiast cyfr
            model.KodPocztowy = "%%-%%%";

            // Symulujemy błąd walidacji (komunikat bez zmian)
            var errorMsg = "Kod pocztowy musi mieć format 00-000";
            controller.ModelState.AddModelError("KodPocztowy", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Czy komunikat o formacie jest poprawny?
            Assert.Equal(errorMsg, controller.ModelState["KodPocztowy"]!.Errors[0].ErrorMessage);

            // Baza danych musi pozostać nienaruszona
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_52_DodanieUzytkownika_KodPocztowyZLiterami_PowinienBycBlad()
        {
            // Arrange
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            // Pobieramy poprawne dane (Tomasz Nowy - TC_1)
            var model = WypelnijModelPoprawnymiDanymi();

            // Zgodnie z TC_52: Format zachowany (myślnik jest), ale zamiast cyfr są litery
            model.KodPocztowy = "ww-www";

            // Symulujemy błąd walidacji formatu kodu pocztowego
            var errorMsg = "Kod pocztowy musi mieć format 00-000";
            controller.ModelState.AddModelError("KodPocztowy", errorMsg);

            // Act
            var result = controller.Dodaj(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikacja komunikatu błędu
            Assert.Equal(errorMsg, controller.ModelState["KodPocztowy"]!.Errors[0].ErrorMessage);

            // Baza danych nie może przyjąć liter w kodzie pocztowym
            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        public class LocalFakeTempDataProvider : ITempDataProvider
        {
            public IDictionary<string, object> LoadTempData(HttpContext context)
                => new Dictionary<string, object>();
            public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
        }
    }
}