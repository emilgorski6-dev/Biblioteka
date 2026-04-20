using Microsoft.AspNetCore.Mvc;
using Biblioteka.Web.Controllers;
using Biblioteka.Web.Models;
using Biblioteka.Web.Data;
using Biblioteka.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Biblioteka.Tests.Controllers
{
    public class AccountControllerTests
    {
        private static class TestData
        {
            public const string Login = "logol";
            public const string Password = "Logol123!";
        }

        private BibliotekaDbContext GetContext()
        {
            var options = new DbContextOptionsBuilder<BibliotekaDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new BibliotekaDbContext(options);
        }

        private void MockAuthentication(AccountController controller)
        {
            var authServiceMock = new Mock<IAuthenticationService>();

            var urlHelperMock = new Mock<IUrlHelper>();

            var services = new ServiceCollection();
            services.AddSingleton(authServiceMock.Object);

            var httpContext = new DefaultHttpContext
            {
                RequestServices = services.BuildServiceProvider()
            };

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            controller.Url = urlHelperMock.Object;
        }

        private void SeedUser(BibliotekaDbContext context)
        {
            context.Uzytkownicy.Add(new Uzytkownik
            {
                Login = TestData.Login,
                HasloHash = TestData.Password,
                Imie = "Test",
                Nazwisko = "User",
                Email = "test@biblioteka.pl",
                Telefon = "000000000",
                Pesel = "00000000000",
                Miejscowosc = "Brak",
                KodPocztowy = "00-000",
                NumerPosesji = "0",
                CzyZablokowany = false
            });
            context.SaveChanges();
        }

        [Fact]
        public async Task TC_L1_Logowanie_PoprawneDane_Sukces()
        {
            using var context = GetContext();
            SeedUser(context);
            var controller = new AccountController(context);

            MockAuthentication(controller);

            var model = new LoginViewModel
            {
                Login = TestData.Login,
                Password = TestData.Password
            };

            var result = await controller.Login(model);

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task TC_L2_Logowanie_NiepoprawnyLogin_Blad()
        {
            using var context = GetContext();
            var controller = new AccountController(context);

            var model = new LoginViewModel
            {
                Login = "logoll",
                Password = TestData.Password
            };

            var result = await controller.Login(model) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
            var errorMessage = controller.ModelState[""]?.Errors[0].ErrorMessage;
            Assert.Equal("Niepoprawny login lub hasło.", errorMessage);
        }
    }
}