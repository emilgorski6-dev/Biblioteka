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
    public class UprawnieniaControllerTests
    {
        private BibliotekaDbContext GetContext()
        {
            var options = new DbContextOptionsBuilder<BibliotekaDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new BibliotekaDbContext(options);
        }

        private Uzytkownik StworzTestowegoUzytkownika(string login)
        {
            return new Uzytkownik
            {
                Login = login,
                Imie = "Bożydar",
                Nazwisko = "Matejko",
                Email = login + "@wp.pl",
                Telefon = "123456789",
                Pesel = "00270345674",
                Miejscowosc = "Poznań",
                KodPocztowy = "12-200",
                NumerPosesji = "5",
                DataUrodzenia = new DateTime(2000, 7, 3),
                Plec = TypPlci.Mezczyzna,
                HasloHash = "test",
                Uprawnienia = new List<Uprawnienie>(),
                CzyZapomniany = false
            };
        }

        [Fact]
        public void TC_U3_NadanieUprawnienia_Bibliotekarz_Sukces()
        {
            using var context = GetContext();
            var uzytkownik = StworzTestowegoUzytkownika("bozydarjp");
            var upr = new Uprawnienie { Id = 2, Nazwa = "Bibliotekarz", Opis = "Test" };

            context.Uzytkownicy.Add(uzytkownik);
            context.Uprawnienia.Add(upr);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), new LocalFakeTempDataProvider());

            var model = new ZapiszUprawnieniaModel
            {
                Login = uzytkownik.Login,
                WybraneRole = new List<string> { "Bibliotekarz" }
            };

            controller.ZapiszUprawnienia(model);

            var userZazy = context.Uzytkownicy
                .Include(u => u.Uprawnienia)
                .First(u => u.Id == uzytkownik.Id);

            Assert.Contains(userZazy.Uprawnienia, u => u.Nazwa == "Bibliotekarz");
        }

        [Fact]
        public void TC_U8_CzescioweOdebranieUprawnien_Sukces()
        {
            using var context = GetContext();
            var uzytkownik = StworzTestowegoUzytkownika("bozydarjp");
            var u1 = new Uprawnienie { Id = 1, Nazwa = "Administrator" };
            var u2 = new Uprawnienie { Id = 2, Nazwa = "Bibliotekarz" };

            context.Uprawnienia.AddRange(u1, u2);
            uzytkownik.Uprawnienia = new List<Uprawnienie> { u1, u2 };
            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), new LocalFakeTempDataProvider());

            var model = new ZapiszUprawnieniaModel
            {
                Login = uzytkownik.Login,
                WybraneRole = new List<string> { "Bibliotekarz" }
            };
            controller.ZapiszUprawnienia(model);

            var userZazy = context.Uzytkownicy
                .Include(u => u.Uprawnienia)
                .First(u => u.Id == uzytkownik.Id);

            Assert.Single(userZazy.Uprawnienia);
            Assert.Equal("Bibliotekarz", userZazy.Uprawnienia.First().Nazwa);
        }

        public class LocalFakeTempDataProvider : ITempDataProvider
        {
            public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
            public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
        }
    }
}