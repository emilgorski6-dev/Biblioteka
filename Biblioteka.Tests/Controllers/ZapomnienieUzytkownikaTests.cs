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
    public class ZapomnienieUzytkownikaTests
    {
        private BibliotekaDbContext GetContext()
        {
            var options = new DbContextOptionsBuilder<BibliotekaDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new BibliotekaDbContext(options);
        }

        [Fact]
        public void TC_103_ZapomnienieUzytkownika_Sukces_DanePowinnyBycNadpisane()
        {
            using var context = GetContext();

            var uzytkownik = new Uzytkownik
            {
                Login = "user_to_forget",
                Imie = "Jan",
                Nazwisko = "Kowalski",
                Pesel = "90010112345",
                Email = "jan@kowalski.pl",
                Telefon = "123456789",
                Miejscowosc = "Łódź",
                KodPocztowy = "90-001",
                NumerPosesji = "10",
                DataUrodzenia = new DateTime(1990, 1, 1),
                Plec = TypPlci.Mezczyzna,
                CzyZapomniany = false // Poprawiona nazwa pola!
            };
            context.Uzytkownicy.Add(uzytkownik);
            context.SaveChanges();

            var controller = new UzytkownicyController(context);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), new LocalFakeTempDataProvider());

            // Act
            var result = controller.Zapomnij(uzytkownik.Id) as RedirectToActionResult;

            // Assert
            var zapomnianyUser = context.Uzytkownicy.Find(uzytkownik.Id);

            Assert.True(zapomnianyUser!.CzyZapomniany); // Sprawdzamy flagę
            Assert.NotEqual("Jan", zapomnianyUser.Imie); // Sprawdzamy czy zamazano dane
            Assert.NotEqual("Kowalski", zapomnianyUser.Nazwisko);
        }
        public class LocalFakeTempDataProvider : ITempDataProvider
        {
            public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
            public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
        }
    }
}