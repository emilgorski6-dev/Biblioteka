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
    public class EdycjaUzytkownikaTests
    {
        private BibliotekaDbContext GetContext()
        {
            var options = new DbContextOptionsBuilder<BibliotekaDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new BibliotekaDbContext(options);
        }

        [Fact]
        public void TC_68_EdycjaUzytkownika_DanePoprawne_PowinienZaktualizowacIZwrocicKomunikat()
        {
            // Arrange
            using var context = GetContext();

            // 1. Tworzymy pierwotnego użytkownika "Arekk"
            var pierwotnyUzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Stary",
                Pesel = "04230312379", // Ten sam PESEL co w nowych danych, żeby uniknąć błędu unikalności
                Email = "stary@wp.pl",
                Telefon = "111111111",
                Miejscowosc = "Łódź",
                KodPocztowy = "90-000",
                NumerPosesji = "1",
                DataUrodzenia = new DateTime(2004, 3, 3),
                Plec = TypPlci.Mezczyzna
            };
            context.Uzytkownicy.Add(pierwotnyUzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Konfiguracja TempData dla komunikatu sukcesu
            var httpContext = new DefaultHttpContext();

            controller.TempData = new TempDataDictionary(httpContext, new LocalFakeTempDataProvider());

            // 2. Przygotowujemy model z nowymi danymi (Adam Nowak)
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = pierwotnyUzytkownik.Id, // Kluczowe: musimy edytować tego konkretnego usera
                Login = "Arekk", // Login zazwyczaj jest zablokowany do edycji
                Imie = "Adam",
                Nazwisko = "Nowak",
                Miejscowosc = "Poznań",
                KodPocztowy = "90-292",
                Ulica = "Nowa",
                NumerPosesji = "5",
                NumerLokalu = "4",
                Pesel = "04230312379",
                DataUrodzenia = new DateTime(2004, 3, 3),
                Plec = TypPlci.Mezczyzna,
                Email = "adam@wp.pl",
                Telefon = "333333333"
            };

            // Act
            var result = controller.Edytuj(modelEdycji) as RedirectToActionResult;

            var uzytkownikPoEdycji = context.Uzytkownicy.Find(pierwotnyUzytkownik.Id);
            Assert.Equal("Adam", uzytkownikPoEdycji!.Imie);
            Assert.Equal("Nowak", uzytkownikPoEdycji.Nazwisko);
            Assert.Equal("Poznań", uzytkownikPoEdycji.Miejscowosc);
            Assert.Equal("adam@wp.pl", uzytkownikPoEdycji.Email);

            // Sprawdzamy komunikat sukcesu z dokumentacji
            var oczekiwanyKomunikat = "Zaktualizowano dane użytkownika (Adam Nowak).";
            Assert.Equal(oczekiwanyKomunikat, controller.TempData["SuccessMessage"]);

            // Sprawdzamy czy przekierowało na Index
            Assert.Equal("Index", result?.ActionName);
        }
        [Fact]
        public void TC_70_EdycjaUzytkownika_UsuniecieImienia_PowinienZwrocicBladWalidacji()
        {
            // Arrange
            using var context = GetContext();

            // Tworzymy istniejącego użytkownika Arekk (z kompletem wymaganych danych)
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };

            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji zgodnie z TC_70 (Imię jest puste)
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "", // To pole czyścimy
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            // Symulujemy, że walidator ASP.NET wykrył brak imienia
            controller.ModelState.AddModelError("Imie", "Imię jest wymagane");

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikacja konkretnego komunikatu z dokumentacji
            Assert.Equal("Imię jest wymagane", controller.ModelState["Imie"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("Arkadiusz", uzytkownikWBazie!.Imie);
        }
        [Fact]
        public void TC_72_EdycjaUzytkownika_UsuniecieNazwiska_PowinienZwrocicBladWalidacji()
        {
            // Arrange
            using var context = GetContext();

            // Tworzymy istniejącego użytkownika Arekk (z kompletem danych)
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };

            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji zgodnie z TC_72 (Nazwisko jest puste)
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "", // Pole puste zgodnie ze scenariuszem
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            controller.ModelState.AddModelError("Nazwisko", "Nazwisko jest wymagane");

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal("Nazwisko jest wymagane", controller.ModelState["Nazwisko"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("Kowalski", uzytkownikWBazie!.Nazwisko);
        }
        [Fact]
        public void TC_73_EdycjaUzytkownika_UsuniecieMiejscowosci_PowinienZwrocicBladWalidacji()
        {
            // Arrange
            using var context = GetContext();

            // Tworzymy istniejącego użytkownika Arekk
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };

            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "", // Pole puste zgodnie ze scenariuszem
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            // Symulujemy wykrycie braku miejscowości przez walidator
            controller.ModelState.AddModelError("Miejscowosc", "Miejscowość jest wymagana");

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikacja komunikatu błędu pod właściwym polem
            Assert.Equal("Miejscowość jest wymagana", controller.ModelState["Miejscowosc"]!.Errors[0].ErrorMessage);

            // Upewniamy się, że w bazie miejscowość nadal brzmi "Łowicz"
            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("Łowicz", uzytkownikWBazie!.Miejscowosc);
        }
        [Fact]
        public void TC_74_EdycjaUzytkownika_UsuniecieKoduPocztowego_PowinienZwrocicBladWalidacji()
        {
            // Arrange
            using var context = GetContext();

            // Tworzymy istniejącego użytkownika Arekk
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400", // Tę wartość zaraz wyczyścimy
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };

            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji zgodnie z TC_74
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "", // Puste pole zgodnie ze scenariuszem
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            // Symulujemy walidację wymagalności kodu pocztowego
            controller.ModelState.AddModelError("KodPocztowy", "Kod pocztowy jest wymagany");

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy komunikat
            Assert.Equal("Kod pocztowy jest wymagany", controller.ModelState["KodPocztowy"]!.Errors[0].ErrorMessage);

            // Sprawdzamy, czy w bazie nadal widnieje stary kod "99-400"
            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("99-400", uzytkownikWBazie!.KodPocztowy);
        }
        [Fact]
        public void TC_75_EdycjaUzytkownika_UsuniecieNumeruPosesji_PowinienZwrocicBladWalidacji()
        {
            // Arrange
            using var context = GetContext();

            // Tworzymy istniejącego użytkownika Arekk
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41", // Tę wartość wyczyścimy w modelu
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };

            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji zgodnie z TC_75
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "", // Puste pole zgodnie ze scenariuszem
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            // Symulujemy walidację wymagalności numeru posesji
            controller.ModelState.AddModelError("NumerPosesji", "Numer posesji jest wymagany");

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikacja komunikatu
            Assert.Equal("Numer posesji jest wymagany", controller.ModelState["NumerPosesji"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("41", uzytkownikWBazie!.NumerPosesji);
        }
        [Fact]
        public void TC_76_EdycjaUzytkownika_UsunieciePesel_PowinienZwrocicBladWalidacji()
        {
            // Arrange
            using var context = GetContext();

            // Tworzymy istniejącego użytkownika Arekk
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };

            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji zgodnie z TC_76
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "", // Puste pole zgodnie ze scenariuszem
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            // Symulujemy walidację wymagalności PESEL
            controller.ModelState.AddModelError("Pesel", "PESEL jest wymagany");

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikacja komunikatu
            Assert.Equal("PESEL jest wymagany", controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            // Sprawdzamy, czy w bazie pozostał stary PESEL
            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("89041412353", uzytkownikWBazie!.Pesel);
        }
        [Fact]
        public void TC_77_EdycjaUzytkownika_UsuniecieDatyUrodzenia_PowinienZwrocicBladWalidacji()
        {
            // Arrange
            using var context = GetContext();

            // Tworzymy istniejącego użytkownika Arekk
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };

            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji zgodnie z TC_77 (Data urodzenia jest null/pusta)
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = null, // Puste pole zgodnie ze scenariuszem
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            controller.ModelState.AddModelError("DataUrodzenia", "Data urodzenia jest wymagana");

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikacja komunikatu
            Assert.Equal("Data urodzenia jest wymagana", controller.ModelState["DataUrodzenia"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal(new DateTime(1989, 4, 14), uzytkownikWBazie!.DataUrodzenia);
        }
        [Fact]
        public void TC_78_EdycjaUzytkownika_UsunieciePlci_PowinienZwrocicBladWalidacji()
        {
            // Arrange
            using var context = GetContext();

            // Tworzymy istniejącego użytkownika Arekk
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };

            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji zgodnie z TC_78 (Płeć ustawiona na null)
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = null, // Puste pole zgodnie ze scenariuszem
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            // Symulujemy walidację wymagalności płci
            controller.ModelState.AddModelError("Plec", "Płeć jest wymagana");

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikacja komunikatu
            Assert.Equal("Płeć jest wymagana", controller.ModelState["Plec"]!.Errors[0].ErrorMessage);

            // Sprawdzamy, czy w bazie pozostała stara wartość (Mezczyzna)
            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal(TypPlci.Mezczyzna, uzytkownikWBazie!.Plec);
        }
        [Fact]
        public void TC_79_EdycjaUzytkownika_UsuniecieEmail_PowinienZwrocicBladWalidacji()
        {
            // Arrange
            using var context = GetContext();

            // Tworzymy istniejącego użytkownika Arekk
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl", // Tę wartość wyczyścimy
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };

            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji zgodnie z TC_79
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "", // Puste pole zgodnie ze scenariuszem
                Telefon = "333333335"
            };

            // Symulujemy walidację wymagalności e-maila
            controller.ModelState.AddModelError("Email", "Email jest wymagany");

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikacja komunikatu
            Assert.Equal("Email jest wymagany", controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            // Sprawdzamy, czy w bazie pozostał stary e-mail
            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("arkadiusz@wp.pl", uzytkownikWBazie!.Email);
        }
        [Fact]
        public void TC_80_EdytujUzytkownika_UsuniecieTelefonu_PowinienZwrocicBladWalidacji()
        {
            // Arrange
            using var context = GetContext();

            // Tworzymy istniejącego użytkownika Arekk
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335", // Tę wartość wyczyścimy
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };

            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji zgodnie z TC_80
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "" // Puste pole zgodnie ze scenariuszem
            };

            // Symulujemy walidację wymagalności telefonu
            controller.ModelState.AddModelError("Telefon", "Telefon jest wymagany");

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikacja komunikatu
            Assert.Equal("Telefon jest wymagany", controller.ModelState["Telefon"]!.Errors[0].ErrorMessage);

            // Sprawdzamy, czy w bazie pozostał stary numer telefonu
            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("333333335", uzytkownikWBazie!.Telefon);
        }
        [Fact]
        public void TC_81_EdycjaUzytkownika_PeselNiezgodnyZData_PowinienZwrocicBladZWalidatora()
        {
            // Arrange
            using var context = GetContext();

            // Arekk w bazie ma poprawne dane (np. rok 1989 w PESEL i dacie)
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };
            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji zgodnie z TC_81:
            // PESEL zaczyna się od 002703 (3 lipca 2000), a data to 14.04.1989
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "00270345674", // Nowy, błędny PESEL
                DataUrodzenia = new DateTime(1989, 4, 14), // Stara data
                Plec = TypPlci.Mezczyzna,
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            // Act
            // NIE dodajemy ręcznie błędu do ModelState. 
            // Chcemy sprawdzić, czy kontroler sam go doda, wywołując PeselValidator.
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy, czy błąd pochodzi z Twojego PeselValidatora
            var oczekiwanyBlad = "Pierwsze sześć cyfr numeru PESEL nie zgadza się z podaną datą urodzenia";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            // Upewniamy się, że w bazie dane NIE zostały zmienione
            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("89041412353", uzytkownikWBazie!.Pesel);
        }
        [Fact]
        public void TC_82_EdycjaUzytkownika_PeselJuzIstnieje_PowinienZwrocicBladUnikalnosci()
        {
            // Arrange
            using var context = GetContext();

            // 1. Dodajemy "Innego Użytkownika", który już ma ten PESEL
            var innyUzytkownik = new Uzytkownik
            {
                Login = "InnyUser",
                Imie = "Jan",
                Nazwisko = "Zajety",
                Pesel = "26240138271",
                Email = "jan@wp.pl",
                Telefon = "111222333",
                Miejscowosc = "Warszawa",
                KodPocztowy = "00-001",
                NumerPosesji = "1",
                DataUrodzenia = new DateTime(2026, 4, 1),
                Plec = TypPlci.Mezczyzna
            };

            // 2. Dodajemy Arka, którego będziemy edytować
            var arekk = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };

            context.Uzytkownicy.AddRange(innyUzytkownik, arekk);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // 3. Przygotowujemy model edycji dla Arka, wpisując mu PESEL Jana
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = arekk.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "26240138271", // PESEL należący do InnyUser
                DataUrodzenia = new DateTime(2026, 4, 1), // Zmieniamy też datę na zgodną z tym PESELEM
                Plec = TypPlci.Mezczyzna,
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikacja komunikatu o duplikacie
            var oczekiwanyBlad = "Ten numer PESEL jest już przypisany do innego użytkownika.";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            // Upewniamy się, że Arek w bazie nadal ma swój stary PESEL
            var arekkWBazie = context.Uzytkownicy.Find(arekk.Id);
            Assert.Equal("89041412353", arekkWBazie!.Pesel);
        }
        [Fact]
        public void TC_83_EdycjaUzytkownika_NiezgodnoscPlci_PowinienZwrocicBladZWalidatora()
        {
            // Arrange
            using var context = GetContext();

            // Tworzymy istniejącego Arka z poprawnymi danymi
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };
            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji zgodnie z TC_83:
            // PESEL: 00270345682 (10. cyfra 8 -> Kobieta)
            // Płeć: Mężczyzna -> Konflikt!
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "00270345682",
                DataUrodzenia = new DateTime(2000, 7, 3), // Data zgodna z nowym PESELem
                Plec = TypPlci.Mezczyzna, // Ale płeć wybrana błędnie
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy czy komunikat jest identyczny z wymaganiem z Twojego TC
            var oczekiwanyBlad = "Przedostatnia cyfra numeru PESEL wskazuje na płeć żeńską, a w formularzu wybrano płeć męską.";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            // Upewniamy się, że w bazie nic się nie zmieniło
            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("89041412353", uzytkownikWBazie!.Pesel);
        }
        [Fact]
        public void TC_84_EdycjaUzytkownika_UsunieciePlci_PowinienZwrocicBladZWalidatora()
        {
            // Arrange
            using var context = GetContext();

            // Arekk istnieje w bazie z poprawnymi danymi (męskimi)
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };
            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji dokładnie wg Twojej listy:
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "00270345674",      // PESEL męski (10. cyfra to 7)
                DataUrodzenia = new DateTime(2000, 7, 3),
                Plec = TypPlci.Kobieta,     // Wybrana płeć żeńska -> BŁĄD
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikacja komunikatu - dokładnie tak jak w Twoim opisie
            var oczekiwanyBlad = "Przedostatnia cyfra numeru PESEL wskazuje na płeć męską, a w formularzu wybrano płeć żeńską.";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            // Upewniamy się, że w bazie dane pozostały stare (nie zapisały się błędne)
            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("89041412353", uzytkownikWBazie!.Pesel);
        }
        [Fact]
        public void TC_85_EdycjaUzytkownika_BlednaCyfraKontrolna_PowinienZwrocicBladZWalidatora()
        {
            // Arrange
            using var context = GetContext();

            // Arekk istnieje w bazie z poprawnymi danymi
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };
            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji wg TC_85:
            // PESEL: 00270345675 (Ostatnia cyfra 5 jest błędna, powinno być 4)
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "00270345675",      // Błędna cyfra kontrolna
                DataUrodzenia = new DateTime(2000, 7, 3),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikacja komunikatu - musi być identyczny z wymaganiem
            var oczekiwanyBlad = "Błędna cyfra kontrolna PESEL.";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            // Upewniamy się, że w bazie pozostał stary, poprawny PESEL
            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("89041412353", uzytkownikWBazie!.Pesel);
        }
        [Fact]
        public void TC_86_EdycjaUzytkownika_EmotikonyWPesel_PowinienZwrocicBladFormatowania()
        {
            // Arrange
            using var context = GetContext();

            // Arekk istnieje w bazie z poprawnymi danymi
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };
            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji wg TC_86:
            // PESEL: 😂😂😂😂😂 (znaki specjalne)
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "😂😂😂😂😂",      // Emotikony zamiast cyfr
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            var oczekiwanyBlad = "Nieprawidłowy format numeru PESEL";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            // Baza musi pozostać nienaruszona
            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("89041412353", uzytkownikWBazie!.Pesel);
        }
        [Fact]
        public void TC_87_EdycjaUzytkownika_ZbytKrotkiPesel_PowinienZwrocicBladFormatowania()
        {
            // Arrange
            using var context = GetContext();

            // Arekk istnieje w bazie
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };
            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji wg TC_87:
            // PESEL: 0027034567 (10 cyfr zamiast 11)
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "0027034567",      // Za krótki PESEL
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy komunikat o błędnej długości
            var oczekiwanyBlad = "Nieprawidłowy format numeru PESEL";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            // Weryfikacja, czy dane w bazie nie drgnęły
            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("89041412353", uzytkownikWBazie!.Pesel);
        }
        [Fact]
        public void TC_88_EdycjaUzytkownika_SymboleWPesel_PowinienZwrocicBladFormatowania()
        {
            // Arrange
            using var context = GetContext();

            // Arekk w bazie danych
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };
            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji wg TC_88:
            // PESEL: %%%%%%%%%%% (symbole specjalne)
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "%%%%%%%%%%%",      // Symbole zamiast cyfr
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy, czy system wyrzucił poprawny komunikat o 11 cyfrach
            var oczekiwanyBlad = "Nieprawidłowy format numeru PESEL";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            // Weryfikacja, czy dane w bazie pozostały bezpieczne
            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("89041412353", uzytkownikWBazie!.Pesel);
        }
        [Fact]
        public void TC_89_EdycjaUzytkownika_LiteryWPesel_PowinienZwrocicBladFormatowania()
        {
            // Arrange
            using var context = GetContext();

            // Tworzymy użytkownika Arekk w bazie
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };
            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji wg TC_89:
            // PESEL: lllllllllll (litery zamiast cyfr)
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "lllllllllll",      // Litery zamiast cyfr
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy komunikat o wymaganych cyfrach
            var oczekiwanyBlad = "Nieprawidłowy format numeru PESEL";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            // Weryfikacja, czy stary PESEL w bazie został nienaruszony
            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("89041412353", uzytkownikWBazie!.Pesel);
        }
        [Fact]
        public void TC_90_EdycjaUzytkownika_PodwojnaMalpaWEmail_PowinienZwrocicBladFormatowania()
        {
            // Arrange
            using var context = GetContext();

            // Arekk w bazie z poprawnym mailem
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };
            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji wg TC_90:
            // E-mail: adam@@wp.pl (błędny format)
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "adam@@wp.pl",      // Nadmiarowy znak @
                Telefon = "333333335"
            };

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Sprawdzamy komunikat o nieprawidłowym formacie
            var oczekiwanyBlad = "Nieprawidłowy format adresu e-mail";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            // Weryfikacja, czy stary e-mail w bazie pozostał bez zmian
            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("arkadiusz@wp.pl", uzytkownikWBazie!.Email);
        }

        [Fact]
        public void TC_91_EdycjaUzytkownika_BrakMalpyWEmail_PowinienZwrocicBladFormatowania()
        {
            // Arrange
            using var context = GetContext();

            // Arekk w bazie z poprawnymi danymi
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };
            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji wg TC_91:
            // E-mail: adamwp.pl (brak znaku @)
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "adamwp.pl",
                Telefon = "333333335"
            };

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikacja komunikatu o błędzie
            var oczekiwanyBlad = "Nieprawidłowy format adresu e-mail";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("333333335", uzytkownikWBazie!.Telefon);
            Assert.Equal("arkadiusz@wp.pl", uzytkownikWBazie.Email);
        }
        [Fact]
        public void TC_92_EdycjaUzytkownika_BrakKropkiWEmail_PowinienZwrocicBladFormatowania()
        {
            // Arrange
            using var context = GetContext();

            // Arekk w bazie danych
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };
            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji wg TC_92:
            // E-mail: adam@wppl (brak kropki w domenie)
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "adam@wppl",      // Brak kropki
                Telefon = "333333335"
            };

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikacja komunikatu o błędzie
            var oczekiwanyBlad = "Nieprawidłowy format adresu e-mail";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            // Baza danych musi pozostać nienaruszona
            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("arkadiusz@wp.pl", uzytkownikWBazie!.Email);
        }
        [Fact]
        public void TC_93_EdycjaUzytkownika_EmailJuzIstnieje_PowinienZwrocicBladUnikalnosci()
        {
            // Arrange
            using var context = GetContext();

            // 1. Dodajemy innego użytkownika, który już używa maila emil.gorski6@gmail.com
            var innyUzytkownik = new Uzytkownik
            {
                Login = "EmilG",
                Imie = "Emil",
                Nazwisko = "Górski",
                Pesel = "95010112345",
                Email = "emil.gorski6@gmail.com",
                Telefon = "999888777",
                Miejscowosc = "Kraków",
                KodPocztowy = "30-001",
                NumerPosesji = "10",
                DataUrodzenia = new DateTime(1995, 1, 1),
                Plec = TypPlci.Mezczyzna
            };

            // 2. Dodajemy Arka, którego będziemy edytować
            var arekk = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };

            context.Uzytkownicy.AddRange(innyUzytkownik, arekk);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // 3. Przygotowujemy model edycji dla Arka, wpisując mu e-mail Emila
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = arekk.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "emil.gorski6@gmail.com", // Zajęty e-mail
                Telefon = "333333335"
            };

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikacja komunikatu o duplikacie
            var oczekiwanyBlad = "Adres email został już zarejestrowany dla innego konta.";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            // Upewniamy się, że Arek w bazie nadal ma swój pierwotny e-mail
            var arekkWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("arkadiusz@wp.pl", arekkWBazie!.Email);
        }
        [Fact]
        public void TC_94_EdycjaUzytkownika_EmotikonyWEmail_PowinienZwrocicBladFormatowania()
        {
            // Arrange
            using var context = GetContext();

            // Arekk istnieje w bazie z poprawnym mailem
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };
            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji wg TC_94:
            // E-mail: 😂😂@😂😂.😂😂
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "😂😂@😂😂.😂😂", // Emotikony w adresie e-mail
                Telefon = "333333335"
            };

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikacja komunikatu o nieprawidłowym formacie
            var oczekiwanyBlad = "Nieprawidłowy format adresu e-mail";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            // Sprawdzamy, czy w bazie nadal widnieje stary, bezpieczny adres e-mail
            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("arkadiusz@wp.pl", uzytkownikWBazie!.Email);
        }
        [Fact]
        public void TC_95_EdycjaUzytkownika_SymboleWEmail_PowinienZwrocicBladFormatowania()
        {
            // Arrange
            using var context = GetContext();

            // Arekk istnieje w bazie
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };
            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji wg TC_95:
            // E-mail: %%@%%.%%
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "%%@%%.%%",      // Symbole specjalne zamiast poprawnej nazwy/domeny
                Telefon = "333333335"
            };

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikacja komunikatu
            var oczekiwanyBlad = "Nieprawidłowy format adresu e-mail";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            // Sprawdzamy, czy dane w bazie pozostały nienaruszone
            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("arkadiusz@wp.pl", uzytkownikWBazie!.Email);
        }
        [Fact]
        public void TC_96_EdycjaUzytkownika_PolskieZnakiWEmail_PowinienZwrocicBladFormatowania()
        {
            // Arrange
            using var context = GetContext();

            // Arekk istnieje w bazie danych
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };
            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // E-mail: arkadiósz@wp.pl (litera 'ó' zamiast 'u')
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiósz@wp.pl",      // Polskie znaki
                Telefon = "333333335"
            };

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikacja komunikatu o nieprawidłowym formacie
            var oczekiwanyBlad = "Nieprawidłowy format adresu e-mail";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            // Baza danych nie powinna zostać zaktualizowana
            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("arkadiusz@wp.pl", uzytkownikWBazie!.Email);
        }
        [Fact]
        public void TC_97_EdycjaUzytkownika_ZbytKrotkiTelefon_PowinienZwrocicBladFormatowania()
        {
            // Arrange
            using var context = GetContext();

            // Arekk istnieje w bazie z poprawnym telefonem (9 cyfr)
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };
            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji wg TC_97:
            // Telefon: 33333333 (8 cyfr)
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "33333333" // Za krótki numer
            };

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikacja komunikatu
            var oczekiwanyBlad = "Numer telefonu musi zawierać dokładnie 9 cyfr";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Telefon"]!.Errors[0].ErrorMessage);

            // Sprawdzamy, czy stary numer w bazie nie został nadpisany
            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("333333335", uzytkownikWBazie!.Telefon);
        }
        [Fact]
        public void TC_98_EdycjaUzytkownika_EmotikonyWTelefonie_PowinienZwrocicBladFormatowania()
        {
            // Arrange
            using var context = GetContext();

            // Arekk istnieje w bazie z poprawnym telefonem
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };
            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji wg TC_98:
            // Telefon: 😂😂😂😂
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "😂😂😂😂" // Emotikony zamiast cyfr
            };

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikacja komunikatu
            var oczekiwanyBlad = "Numer telefonu musi zawierać dokładnie 9 cyfr";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Telefon"]!.Errors[0].ErrorMessage);

            // Sprawdzamy, czy w bazie nadal jest bezpieczny stary numer
            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("333333335", uzytkownikWBazie!.Telefon);
        }
        [Fact]
        public void TC_99_EdycjaUzytkownika_LiteryWTelefonie_PowinienZwrocicBladFormatowania()
        {
            // Arrange
            using var context = GetContext();

            // Arekk istnieje w bazie z poprawnym numerem
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };
            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji wg TC_99:
            // Telefon: lllllllll (9 liter zamiast cyfr)
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "lllllllll" // Litery zamiast cyfr
            };

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikacja komunikatu
            var oczekiwanyBlad = "Numer telefonu musi zawierać dokładnie 9 cyfr";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Telefon"]!.Errors[0].ErrorMessage);

            // Upewniamy się, że w bazie pozostał stary numer
            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("333333335", uzytkownikWBazie!.Telefon);
        }
        [Fact]
        public void TC_100_EdycjaUzytkownika_SymboleWTelefonie_PowinienZwrocicBladFormatowania()
        {
            // Arrange
            using var context = GetContext();

            // Użytkownik Arekk w bazie
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };
            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji wg TC_100:
            // Telefon: %%%%%%%%% (9 symboli zamiast cyfr)
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "%%%%%%%%%" // Symbole specjalne
            };

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikacja komunikatu błędu
            var oczekiwanyBlad = "Numer telefonu musi zawierać dokładnie 9 cyfr";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Telefon"]!.Errors[0].ErrorMessage);

            // Sprawdzenie, czy dane w bazie nie zostały zmienione
            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("333333335", uzytkownikWBazie!.Telefon);
        }
        [Fact]
        public void TC_101_EdycjaUzytkownika_TelefonJuzIstnieje_PowinienZwrocicBladUnikalnosci()
        {
            // Arrange
            using var context = GetContext();

            // 1. Dodajemy innego użytkownika, który już ma numer 123456789
            var innyUzytkownik = new Uzytkownik
            {
                Login = "Jan_Z",
                Imie = "Jan",
                Nazwisko = "Zajety",
                Pesel = "90010112345",
                Email = "jan@zajety.pl",
                Telefon = "123456789",
                Miejscowosc = "Warszawa",
                KodPocztowy = "00-001",
                NumerPosesji = "1",
                DataUrodzenia = new DateTime(1990, 1, 1),
                Plec = TypPlci.Mezczyzna
            };

            var arekk = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };

            context.Uzytkownicy.AddRange(innyUzytkownik, arekk);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // 3. Przygotowujemy model edycji dla Arka, wpisując mu numer Jana
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = arekk.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "123456789" // Zajęty numer telefonu
            };

            // Act
            var result = controller.Edytuj(modelEdycji) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            // Weryfikacja komunikatu o duplikacie
            var oczekiwanyBlad = "Ten numer telefonu jest już przypisany do innego użytkownika.";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Telefon"]!.Errors[0].ErrorMessage);

            // Upewniamy się, że Arek w bazie nadal ma swój stary numer
            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("333333335", uzytkownikWBazie!.Telefon);
        }
        [Fact]
        public void TC_102_EdycjaUzytkownika_DataZPrzyszlosci_PowinienZwrocicBladWalidacji()
        {
            // Arrange
            using var context = GetContext();

            // Arekk istnieje w bazie
            var uzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "89041412353",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Ulica = "Bobra",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna
            };
            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);

            // Przygotowujemy model edycji wg TC_102:
            // PESEL: 27240648214 (zgodny z rokiem 2027)
            // Data ur.: 06.04.2027 (Przyszłość względem kwietnia 2026)
            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "27240648214",
                DataUrodzenia = new DateTime(2027, 4, 6),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

        }

            public class LocalFakeTempDataProvider : ITempDataProvider
        {
            public IDictionary<string, object> LoadTempData(HttpContext context)
                => new Dictionary<string, object>();
            public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
        }
    }
}