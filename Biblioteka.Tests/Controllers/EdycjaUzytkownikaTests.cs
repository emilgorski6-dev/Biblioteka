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
            using var context = GetContext();

            var pierwotnyUzytkownik = new Uzytkownik
            {
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Stary",
                Pesel = "04230312379",
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

            var httpContext = new DefaultHttpContext();

            controller.TempData = new TempDataDictionary(httpContext, new LocalFakeTempDataProvider());

            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = pierwotnyUzytkownik.Id,
                Login = "Arekk",
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

            var result = controller.Edytuj(modelEdycji) as RedirectToActionResult;

            var uzytkownikPoEdycji = context.Uzytkownicy.Find(pierwotnyUzytkownik.Id);
            Assert.Equal("Adam", uzytkownikPoEdycji!.Imie);
            Assert.Equal("Nowak", uzytkownikPoEdycji.Nazwisko);
            Assert.Equal("Poznań", uzytkownikPoEdycji.Miejscowosc);
            Assert.Equal("adam@wp.pl", uzytkownikPoEdycji.Email);

            var oczekiwanyKomunikat = "Zaktualizowano dane użytkownika (Adam Nowak).";
            Assert.Equal(oczekiwanyKomunikat, controller.TempData["SuccessMessage"]);

            Assert.Equal("Index", result?.ActionName);
        }
        [Fact]
        public void TC_70_EdycjaUzytkownika_UsuniecieImienia_PowinienZwrocicBladWalidacji()
        {
            using var context = GetContext();

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
                Imie = "",
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

            controller.ModelState.AddModelError("Imie", "Imię jest wymagane");

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal("Imię jest wymagane", controller.ModelState["Imie"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("Arkadiusz", uzytkownikWBazie!.Imie);
        }
        [Fact]
        public void TC_72_EdycjaUzytkownika_UsuniecieNazwiska_PowinienZwrocicBladWalidacji()
        {
            using var context = GetContext();

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
                Nazwisko = "",
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
            using var context = GetContext();

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
                Miejscowosc = "",
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

            controller.ModelState.AddModelError("Miejscowosc", "Miejscowość jest wymagana");

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal("Miejscowość jest wymagana", controller.ModelState["Miejscowosc"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("Łowicz", uzytkownikWBazie!.Miejscowosc);
        }
        [Fact]
        public void TC_74_EdycjaUzytkownika_UsuniecieKoduPocztowego_PowinienZwrocicBladWalidacji()
        {
            using var context = GetContext();

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
                Miejscowosc = "Łowicz",
                KodPocztowy = "",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            controller.ModelState.AddModelError("KodPocztowy", "Kod pocztowy jest wymagany");

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal("Kod pocztowy jest wymagany", controller.ModelState["KodPocztowy"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("99-400", uzytkownikWBazie!.KodPocztowy);
        }
        [Fact]
        public void TC_75_EdycjaUzytkownika_UsuniecieNumeruPosesji_PowinienZwrocicBladWalidacji()
        {
            using var context = GetContext();

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
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            controller.ModelState.AddModelError("NumerPosesji", "Numer posesji jest wymagany");

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal("Numer posesji jest wymagany", controller.ModelState["NumerPosesji"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("41", uzytkownikWBazie!.NumerPosesji);
        }
        [Fact]
        public void TC_76_EdycjaUzytkownika_UsunieciePesel_PowinienZwrocicBladWalidacji()
        {
            using var context = GetContext();

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
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            controller.ModelState.AddModelError("Pesel", "PESEL jest wymagany");

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal("PESEL jest wymagany", controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("89041412353", uzytkownikWBazie!.Pesel);
        }
        [Fact]
        public void TC_77_EdycjaUzytkownika_UsuniecieDatyUrodzenia_PowinienZwrocicBladWalidacji()
        {
            using var context = GetContext();

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
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = null,
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            controller.ModelState.AddModelError("DataUrodzenia", "Data urodzenia jest wymagana");

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal("Data urodzenia jest wymagana", controller.ModelState["DataUrodzenia"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal(new DateTime(1989, 4, 14), uzytkownikWBazie!.DataUrodzenia);
        }
        [Fact]
        public void TC_78_EdycjaUzytkownika_UsunieciePlci_PowinienZwrocicBladWalidacji()
        {
            using var context = GetContext();

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
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = null,
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            controller.ModelState.AddModelError("Plec", "Płeć jest wymagana");

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal("Płeć jest wymagana", controller.ModelState["Plec"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal(TypPlci.Mezczyzna, uzytkownikWBazie!.Plec);
        }
        [Fact]
        public void TC_79_EdycjaUzytkownika_UsuniecieEmail_PowinienZwrocicBladWalidacji()
        {
            using var context = GetContext();

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
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "",
                Telefon = "333333335"
            };

            controller.ModelState.AddModelError("Email", "Email jest wymagany");

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal("Email jest wymagany", controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("arkadiusz@wp.pl", uzytkownikWBazie!.Email);
        }
        [Fact]
        public void TC_80_EdytujUzytkownika_UsuniecieTelefonu_PowinienZwrocicBladWalidacji()
        {
            using var context = GetContext();

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
                Telefon = ""
            };

            controller.ModelState.AddModelError("Telefon", "Telefon jest wymagany");

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            Assert.Equal("Telefon jest wymagany", controller.ModelState["Telefon"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("333333335", uzytkownikWBazie!.Telefon);
        }
        [Fact]
        public void TC_81_EdycjaUzytkownika_PeselNiezgodnyZData_PowinienZwrocicBladZWalidatora()
        {
            using var context = GetContext();

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

            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "00270345674",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            var oczekiwanyBlad = "Pierwsze sześć cyfr numeru PESEL nie zgadza się z podaną datą urodzenia";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("89041412353", uzytkownikWBazie!.Pesel);
        }
        [Fact]
        public void TC_82_EdycjaUzytkownika_PeselJuzIstnieje_PowinienZwrocicBladUnikalnosci()
        {
            using var context = GetContext();

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

            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = arekk.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "26240138271",
                DataUrodzenia = new DateTime(2026, 4, 1),
                Plec = TypPlci.Mezczyzna,
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            var oczekiwanyBlad = "Ten numer PESEL jest już przypisany do innego użytkownika.";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            var arekkWBazie = context.Uzytkownicy.Find(arekk.Id);
            Assert.Equal("89041412353", arekkWBazie!.Pesel);
        }
        [Fact]
        public void TC_83_EdycjaUzytkownika_NiezgodnoscPlci_PowinienZwrocicBladZWalidatora()
        {
            using var context = GetContext();

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

            var modelEdycji = new EdytujUzytkownikaViewModel
            {
                Id = uzytkownik.Id,
                Login = "Arekk",
                Imie = "Arkadiusz",
                Nazwisko = "Kowalski",
                Pesel = "00270345682",
                DataUrodzenia = new DateTime(2000, 7, 3),
                Plec = TypPlci.Mezczyzna,
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                NumerPosesji = "41",
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            var oczekiwanyBlad = "Przedostatnia cyfra numeru PESEL wskazuje na płeć żeńską, a w formularzu wybrano płeć męską.";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("89041412353", uzytkownikWBazie!.Pesel);
        }
        [Fact]
        public void TC_84_EdycjaUzytkownika_UsunieciePlci_PowinienZwrocicBladZWalidatora()
        {
            using var context = GetContext();

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
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "00270345674",
                DataUrodzenia = new DateTime(2000, 7, 3),
                Plec = TypPlci.Kobieta,
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            var oczekiwanyBlad = "Przedostatnia cyfra numeru PESEL wskazuje na płeć męską, a w formularzu wybrano płeć żeńską.";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("89041412353", uzytkownikWBazie!.Pesel);
        }
        [Fact]
        public void TC_85_EdycjaUzytkownika_BlednaCyfraKontrolna_PowinienZwrocicBladZWalidatora()
        {
            using var context = GetContext();

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
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "00270345675",
                DataUrodzenia = new DateTime(2000, 7, 3),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            var oczekiwanyBlad = "Błędna cyfra kontrolna PESEL.";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("89041412353", uzytkownikWBazie!.Pesel);
        }
        [Fact]
        public void TC_86_EdycjaUzytkownika_EmotikonyWPesel_PowinienZwrocicBladFormatowania()
        {
            using var context = GetContext();

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
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "😂😂😂😂😂",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            var oczekiwanyBlad = "Nieprawidłowy format numeru PESEL";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.Find(uzytkownik.Id);
            Assert.Equal("89041412353", uzytkownikWBazie!.Pesel);
        }
        [Fact]
        public void TC_87_EdycjaUzytkownika_ZbytKrotkiPesel_PowinienZwrocicBladFormatowania()
        {
            using var context = GetContext();

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
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "0027034567",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            var oczekiwanyBlad = "Nieprawidłowy format numeru PESEL";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("89041412353", uzytkownikWBazie!.Pesel);
        }
        [Fact]
        public void TC_88_EdycjaUzytkownika_SymboleWPesel_PowinienZwrocicBladFormatowania()
        {
            using var context = GetContext();

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
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "%%%%%%%%%%%",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            var oczekiwanyBlad = "Nieprawidłowy format numeru PESEL";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("89041412353", uzytkownikWBazie!.Pesel);
        }
        [Fact]
        public void TC_89_EdycjaUzytkownika_LiteryWPesel_PowinienZwrocicBladFormatowania()
        {
            using var context = GetContext();

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
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "lllllllllll",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "333333335"
            };

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            var oczekiwanyBlad = "Nieprawidłowy format numeru PESEL";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Pesel"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("89041412353", uzytkownikWBazie!.Pesel);
        }
        [Fact]
        public void TC_90_EdycjaUzytkownika_PodwojnaMalpaWEmail_PowinienZwrocicBladFormatowania()
        {
            using var context = GetContext();

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
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "adam@@wp.pl",
                Telefon = "333333335"
            };

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            var oczekiwanyBlad = "Nieprawidłowy format adresu e-mail";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("arkadiusz@wp.pl", uzytkownikWBazie!.Email);
        }

        [Fact]
        public void TC_91_EdycjaUzytkownika_BrakMalpyWEmail_PowinienZwrocicBladFormatowania()
        {
            using var context = GetContext();

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

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            var oczekiwanyBlad = "Nieprawidłowy format adresu e-mail";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("333333335", uzytkownikWBazie!.Telefon);
            Assert.Equal("arkadiusz@wp.pl", uzytkownikWBazie.Email);
        }
        [Fact]
        public void TC_92_EdycjaUzytkownika_BrakKropkiWEmail_PowinienZwrocicBladFormatowania()
        {
            using var context = GetContext();

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
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "adam@wppl",
                Telefon = "333333335"
            };

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            var oczekiwanyBlad = "Nieprawidłowy format adresu e-mail";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("arkadiusz@wp.pl", uzytkownikWBazie!.Email);
        }
        [Fact]
        public void TC_93_EdycjaUzytkownika_EmailJuzIstnieje_PowinienZwrocicBladUnikalnosci()
        {
            using var context = GetContext();

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
                Email = "emil.gorski6@gmail.com",
                Telefon = "333333335"
            };

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            var oczekiwanyBlad = "Adres email został już zarejestrowany dla innego konta.";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            var arekkWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("arkadiusz@wp.pl", arekkWBazie!.Email);
        }
        [Fact]
        public void TC_94_EdycjaUzytkownika_EmotikonyWEmail_PowinienZwrocicBladFormatowania()
        {
            using var context = GetContext();

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
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "😂😂@😂😂.😂😂",
                Telefon = "333333335"
            };

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            var oczekiwanyBlad = "Nieprawidłowy format adresu e-mail";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("arkadiusz@wp.pl", uzytkownikWBazie!.Email);
        }
        [Fact]
        public void TC_95_EdycjaUzytkownika_SymboleWEmail_PowinienZwrocicBladFormatowania()
        {
            using var context = GetContext();

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
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "%%@%%.%%",
                Telefon = "333333335"
            };

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            var oczekiwanyBlad = "Nieprawidłowy format adresu e-mail";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("arkadiusz@wp.pl", uzytkownikWBazie!.Email);
        }
        [Fact]
        public void TC_96_EdycjaUzytkownika_PolskieZnakiWEmail_PowinienZwrocicBladFormatowania()
        {
            using var context = GetContext();

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
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiósz@wp.pl",
                Telefon = "333333335"
            };

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            var oczekiwanyBlad = "Nieprawidłowy format adresu e-mail";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Email"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("arkadiusz@wp.pl", uzytkownikWBazie!.Email);
        }
        [Fact]
        public void TC_97_EdycjaUzytkownika_ZbytKrotkiTelefon_PowinienZwrocicBladFormatowania()
        {
            using var context = GetContext();

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
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "33333333"
            };

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            var oczekiwanyBlad = "Numer telefonu musi zawierać dokładnie 9 cyfr";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Telefon"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("333333335", uzytkownikWBazie!.Telefon);
        }
        [Fact]
        public void TC_98_EdycjaUzytkownika_EmotikonyWTelefonie_PowinienZwrocicBladFormatowania()
        {
            using var context = GetContext();

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
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "😂😂😂😂"
            };

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            var oczekiwanyBlad = "Numer telefonu musi zawierać dokładnie 9 cyfr";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Telefon"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("333333335", uzytkownikWBazie!.Telefon);
        }
        [Fact]
        public void TC_99_EdycjaUzytkownika_LiteryWTelefonie_PowinienZwrocicBladFormatowania()
        {
            using var context = GetContext();

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
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "lllllllll"
            };

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            var oczekiwanyBlad = "Numer telefonu musi zawierać dokładnie 9 cyfr";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Telefon"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("333333335", uzytkownikWBazie!.Telefon);
        }
        [Fact]
        public void TC_100_EdycjaUzytkownika_SymboleWTelefonie_PowinienZwrocicBladFormatowania()
        {
            using var context = GetContext();

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
                Miejscowosc = "Łowicz",
                KodPocztowy = "99-400",
                Ulica = "Bobra",
                NumerPosesji = "41",
                NumerLokalu = "2",
                Pesel = "89041412353",
                DataUrodzenia = new DateTime(1989, 4, 14),
                Plec = TypPlci.Mezczyzna,
                Email = "arkadiusz@wp.pl",
                Telefon = "%%%%%%%%%"
            };

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            var oczekiwanyBlad = "Numer telefonu musi zawierać dokładnie 9 cyfr";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Telefon"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("333333335", uzytkownikWBazie!.Telefon);
        }
        [Fact]
        public void TC_101_EdycjaUzytkownika_TelefonJuzIstnieje_PowinienZwrocicBladUnikalnosci()
        {
            using var context = GetContext();

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
                Telefon = "123456789"
            };

            var result = controller.Edytuj(modelEdycji) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);

            var oczekiwanyBlad = "Ten numer telefonu jest już przypisany do innego użytkownika.";
            Assert.Equal(oczekiwanyBlad, controller.ModelState["Telefon"]!.Errors[0].ErrorMessage);

            var uzytkownikWBazie = context.Uzytkownicy.FirstOrDefault(u => u.Login == "Arekk");
            Assert.Equal("333333335", uzytkownikWBazie!.Telefon);
        }
        [Fact]
        public void TC_102_EdycjaUzytkownika_DataZPrzyszlosci_PowinienZwrocicBladWalidacji()
        {
            using var context = GetContext();

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