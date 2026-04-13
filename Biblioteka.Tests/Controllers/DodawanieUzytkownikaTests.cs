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
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var httpContext = new DefaultHttpContext();

            var tempData = new TempDataDictionary(httpContext, new LocalFakeTempDataProvider());
            controller.TempData = tempData;

            var model = WypelnijModelTomaszNowy();

            var result = controller.Dodaj(model) as RedirectToActionResult;

            Assert.Equal(1, context.Uzytkownicy.Count());

            var successMsg = "Utworzono konto użytkownika (Tomasz Nowy).";
            Assert.Equal(successMsg, controller.TempData["SuccessMessage"]);

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public void TC_3_DodanieUzytkownika_PustePoleLogin_PowinnoWylapacBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);
            var model = WypelnijModelPoprawnymiDanymi();
            model.Login = "";

            controller.ModelState.AddModelError("Login", "Login jest wymagany");

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal("Login jest wymagany", controller.ModelState["Login"]!.Errors[0].ErrorMessage);
        }

        [Fact]
        public void TC_4_DodanieUzytkownika_PustePoleImie_PowinnoWylapacBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);
            var model = WypelnijModelPoprawnymiDanymi();
            model.Imie = "";

            controller.ModelState.AddModelError("Imie", "Imię jest wymagane");

            var result = controller.Dodaj(model) as ViewResult;

            Assert.False(controller.ModelState.IsValid);
            Assert.Equal("Imię jest wymagane", controller.ModelState["Imie"]!.Errors[0].ErrorMessage);
        }

        [Fact]
        public void TC_5_DodanieUzytkownika_PustePoleNazwisko_PowinnoWylapacBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);
            var model = WypelnijModelPoprawnymiDanymi();
            model.Nazwisko = "";

            controller.ModelState.AddModelError("Nazwisko", "Nazwisko jest wymagane");

            var result = controller.Dodaj(model) as ViewResult;

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
            using var context = GetContext();
            var controller = new UzytkownicyController(context);
            var model = WypelnijModelPoprawnymiDanymi();
            model.NumerPosesji = "";

            controller.ModelState.AddModelError("NumerPosesji", "Numer posesji jest wymagany");

            var result = controller.Dodaj(model) as ViewResult;

            Assert.False(controller.ModelState.IsValid);
            Assert.Equal("Numer posesji jest wymagany", controller.ModelState["NumerPosesji"]!.Errors[0].ErrorMessage);
        }

        [Fact]
        public void TC_9_DodanieUzytkownika_BrakPesel_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);
            var model = WypelnijModelPoprawnymiDanymi();
            model.Pesel = "";

            controller.ModelState.AddModelError("Pesel", "Numer PESEL jest wymagany");

            var result = controller.Dodaj(model) as ViewResult;

            Assert.False(controller.ModelState.IsValid);
            Assert.Equal("Numer PESEL jest wymagany", controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);
        }

        [Fact]
        public void TC_10_DodanieUzytkownika_BrakDatyUrodzenia_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);
            var model = WypelnijModelPoprawnymiDanymi();

            controller.ModelState.AddModelError("DataUrodzenia", "Data urodzenia jest wymagana");

            var result = controller.Dodaj(model) as ViewResult;

            Assert.False(controller.ModelState.IsValid);
            Assert.Equal("Data urodzenia jest wymagana", controller.ModelState["DataUrodzenia"]!.Errors[0].ErrorMessage);
        }

        [Fact]
        public void TC_11_DodanieUzytkownika_BrakPlci_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);
            var model = WypelnijModelPoprawnymiDanymi();

            controller.ModelState.AddModelError("Plec", "Płeć jest wymagana");

            var result = controller.Dodaj(model) as ViewResult;

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
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

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
                DataUrodzenia = default,
                Plec = default
            };

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

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(11, controller.ModelState.ErrorCount);
        }
        [Fact]
        public void TC_15_DodanieUzytkownika_LoginZaKrotki_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);
            var model = WypelnijModelPoprawnymiDanymi();
            model.Login = "b";

            controller.ModelState.AddModelError("Login", "Login musi mieć od 3 do 20 znaków");

            var result = controller.Dodaj(model) as ViewResult;

            Assert.Equal("Login musi mieć od 3 do 20 znaków", controller.ModelState["Login"]!.Errors[0].ErrorMessage);
        }
        [Fact]
        public void TC_16_DodanieUzytkownika_DuplikatPesel_PowinienWyswietlicBladBazy()
        {
            using var context = GetContext();

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
                Pesel = "26240138271",
                DataUrodzenia = new DateTime(2026, 4, 1),
                Plec = TypPlci.Mezczyzna,
                Email = "bozydar@wp.pl",
                Telefon = "666666666"
            };

            controller.ModelState.AddModelError("Pesel", "Ten numer PESEL jest już w bazie.");

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal("Ten numer PESEL jest już w bazie.", controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);
        }
        [Fact]
        public void TC_21_DodanieUzytkownika_PeselZaKrotki_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.Pesel = "0027034567";

            var errorMsg = "Numer PESEL musi składać się z 11 cyfr.";
            controller.ModelState.AddModelError("Pesel", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_22_DodanieUzytkownika_PeselZEmotikonami_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.Pesel = "😆😆😆😆😆";

            var errorMsg = "Numer PESEL musi składać się z 11 cyfr.";
            controller.ModelState.AddModelError("Pesel", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_23_DodanieUzytkownika_PeselZLiterami_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.Pesel = "lllllllllll";

            var errorMsg = "Numer PESEL musi składać się z 11 cyfr.";
            controller.ModelState.AddModelError("Pesel", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_24_DodanieUzytkownika_PeselZSymbolami_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.Pesel = "%%%%%%%%%%%";

            var errorMsg = "Numer PESEL musi składać się z 11 cyfr.";
            controller.ModelState.AddModelError("Pesel", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_25_DodanieUzytkownika_EmailZPodwojnymMalpa_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.Email = "bozydar@@wp.pl";

            var errorMsg = "Nieprawidłowy format adresu e-mail";
            controller.ModelState.AddModelError("Email", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_26_DodanieUzytkownika_EmailBezKropkiWDomenie_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.Email = "bozydar@wppl";

            var errorMsg = "Nieprawidłowy format adresu e-mail";
            controller.ModelState.AddModelError("Email", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_27_DodanieUzytkownika_EmailJuzIstnieje_PowinienBycBladUnikalnosci()
        {
            using var context = GetContext();

            context.Uzytkownicy.Add(new Uzytkownik
            {
                Login = "pierwszy.uzytkownik",
                Imie = "Emil",
                Nazwisko = "Górski",
                Email = "emil.gorski6@gmail.com",
                Telefon = "999888777",
                Pesel = "11111111111",
                Miejscowosc = "Warszawa",
                KodPocztowy = "00-001",
                NumerPosesji = "1",
                DataUrodzenia = new DateTime(1990, 5, 10),
                Plec = TypPlci.Mezczyzna
            });
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();
            model.Email = "emil.gorski6@gmail.com";

            var errorMsg = "Ten adres e-mail jest już w bazie.";
            controller.ModelState.AddModelError("Email", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            Assert.Equal(1, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_28_DodanieUzytkownika_EmailZEmotikonami_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.Email = "😆😆😆@😆😆.😆😆";

            var errorMsg = "Nieprawidłowy format adresu e-mail";
            controller.ModelState.AddModelError("Email", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_29_DodanieUzytkownika_EmailBezMalpy_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.Email = "bozydarwp.pl";

            var errorMsg = "Nieprawidłowy format adresu e-mail";
            controller.ModelState.AddModelError("Email", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_30_DodanieUzytkownika_EmailZPolskimiZnakami_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.Email = "bożydar@wp.pl";

            var errorMsg = "Nieprawidłowy format adresu e-mail";
            controller.ModelState.AddModelError("Email", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_31_DodanieUzytkownika_LoginZEmotikonami_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.Login = "😆😆😆😆😆";

            var errorMsg = "Login może zawierać tylko litery, cyfry i podkreślnik";
            controller.ModelState.AddModelError("Login", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Login"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_32_DodanieUzytkownika_DuplikatLoginu_PowinienBycBlad()
        {
            using var context = GetContext();

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

            var errorMsg = "Podany login jest już zajęty.";
            controller.ModelState.AddModelError("Login", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Login"]!.Errors[0].ErrorMessage);

            Assert.Equal(1, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_33_DodanieUzytkownika_LoginZPolskimiZnakami_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.Login = "bożydarjp";

            var errorMsg = "Login może zawierać tylko litery, cyfry i podkreślnik";
            controller.ModelState.AddModelError("Login", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Login"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_34_DodanieUzytkownika_LoginZSamychPodkreslnikow_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.Login = "____________________";

            var errorMsg = "Login może zawierać tylko litery, cyfry i podkreślnik";
            controller.ModelState.AddModelError("Login", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Login"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_35_DodanieUzytkownika_TelefonZaKrotki_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.Telefon = "66666666";

            var errorMsg = "Numer telefonu musi składać się z dokładnie 9 cyfr";
            controller.ModelState.AddModelError("Telefon", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Telefon"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_36_DodanieUzytkownika_TelefonZLiterami_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.Telefon = "lllllllll";

            var errorMsg = "Numer telefonu musi składać się z dokładnie 9 cyfr";
            controller.ModelState.AddModelError("Telefon", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Telefon"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_37_DodanieUzytkownika_TelefonZSymbolami_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.Telefon = "%%%%%%%%%";

            var errorMsg = "Numer telefonu musi składać się z dokładnie 9 cyfr";
            controller.ModelState.AddModelError("Telefon", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Telefon"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_38_DodanieUzytkownika_TelefonZEmotikonami_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.Telefon = "😂😂😂😂";

            var errorMsg = "Numer telefonu musi składać się z dokładnie 9 cyfr";
            controller.ModelState.AddModelError("Telefon", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Telefon"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_39_DodanieUzytkownika_DuplikatTelefonu_PowinienBycBlad()
        {
            using var context = GetContext();

            context.Uzytkownicy.Add(new Uzytkownik
            {
                Id = 111,
                Login = "uzytkownik_z_telefonem",
                Imie = "Jan",
                Nazwisko = "Kowalski",
                Email = "jan@kowalski.pl",
                Telefon = "675123343",
                Pesel = "80010112345",
                Miejscowosc = "Kraków",
                KodPocztowy = "30-001",
                NumerPosesji = "10",
                DataUrodzenia = new DateTime(1980, 1, 1),
                Plec = TypPlci.Mezczyzna
            });
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();
            model.Telefon = "675123343";

            var errorMsg = "Ten numer telefonu jest już przypisany do innego użytkownika.";
            controller.ModelState.AddModelError("Telefon", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Telefon"]!.Errors[0].ErrorMessage);

            Assert.Equal(1, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_40_DodanieUzytkownika_DataUrodzeniaZPrzyszlosci_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.DataUrodzenia = new DateTime(2031, 6, 18);
            model.Pesel = "31261812341";

            var errorMsg = "Data urodzenia nie może być z przyszłości.";
            controller.ModelState.AddModelError("DataUrodzenia", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["DataUrodzenia"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_41_DodanieUzytkownika_ImieZEmotikonami_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.Imie = "😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂";

            var errorMsg = "Imię może zawierać tylko litery.";
            controller.ModelState.AddModelError("Imie", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Imie"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_42_DodanieUzytkownika_ImieZSymbolami_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.Imie = "%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%";

            var errorMsg = "Imię może zawierać tylko litery";
            controller.ModelState.AddModelError("Imie", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Imie"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_43_DodanieUzytkownika_ImieZSamymiCyframi_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.Imie = "77777777777777777777777777777777777777777777777777";

            var errorMsg = "Imię może zawierać tylko litery";
            controller.ModelState.AddModelError("Imie", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Imie"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_44_DodanieUzytkownika_NazwiskoZEmotikonami_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.Nazwisko = "😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂😂";

            var errorMsg = "Nazwisko może zawierać tylko litery i myślnik.";
            controller.ModelState.AddModelError("Nazwisko", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Nazwisko"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_45_DodanieUzytkownika_NazwiskoZSymbolami_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.Nazwisko = "%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%";

            var errorMsg = "Nazwisko może zawierać tylko litery i myślnik.";
            controller.ModelState.AddModelError("Nazwisko", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Nazwisko"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_46_DodanieUzytkownika_NazwiskoZSamymiCyframi_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.Nazwisko = "77777777777777777777777777777777777777777777777777";

            var errorMsg = "Nazwisko może zawierać tylko litery i myślnik.";
            controller.ModelState.AddModelError("Nazwisko", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Nazwisko"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_47_DodanieUzytkownika_NazwiskoZNadmiarowymiMyslnikami_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.Nazwisko = "Matejko------";

            var errorMsg = "Nazwisko może zawierać tylko litery i myślnik.";
            controller.ModelState.AddModelError("Nazwisko", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Nazwisko"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_48_DodanieUzytkownika_NazwiskoZSamychMyslnikow_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.Nazwisko = "--------------------------------------------------";

            var errorMsg = "Nazwisko może zawierać tylko litery i myślnik.";
            controller.ModelState.AddModelError("Nazwisko", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["Nazwisko"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_49_DodanieUzytkownika_KodPocztowyBezMyslnika_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.KodPocztowy = "66666666";

            var errorMsg = "Kod pocztowy musi mieć format 00-000";
            controller.ModelState.AddModelError("KodPocztowy", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["KodPocztowy"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_50_DodanieUzytkownika_KodPocztowyZEmotikonami_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.KodPocztowy = "😂😂-😂😂😂";

            var errorMsg = "Kod pocztowy musi mieć format 00-000";
            controller.ModelState.AddModelError("KodPocztowy", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["KodPocztowy"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_51_DodanieUzytkownika_KodPocztowyZSymbolami_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.KodPocztowy = "%%-%%%";

            var errorMsg = "Kod pocztowy musi mieć format 00-000";
            controller.ModelState.AddModelError("KodPocztowy", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["KodPocztowy"]!.Errors[0].ErrorMessage);

            Assert.Equal(0, context.Uzytkownicy.Count());
        }
        [Fact]
        public void TC_52_DodanieUzytkownika_KodPocztowyZLiterami_PowinienBycBlad()
        {
            using var context = GetContext();
            var controller = new UzytkownicyController(context);

            var model = WypelnijModelPoprawnymiDanymi();

            model.KodPocztowy = "ww-www";

            var errorMsg = "Kod pocztowy musi mieć format 00-000";
            controller.ModelState.AddModelError("KodPocztowy", errorMsg);

            var result = controller.Dodaj(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal(errorMsg, controller.ModelState["KodPocztowy"]!.Errors[0].ErrorMessage);

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