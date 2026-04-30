using Biblioteka.Web.Data;
using Biblioteka.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BibliotekaDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// REJESTRACJA SERWISÓW
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// WBUDOWANA OBSŁUGA LOGOWANIA/WYLOGOWANIA
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
    });

// KONFIGURACJA KOMUNIKATÓW WALIDACJI (ZRK-01 pkt 6)
builder.Services.AddControllersWithViews()
    .AddMvcOptions(options =>
    {
        // Obsługa błędu formatu danych liczbowych (wprowadzenie liter zamiast cyfr)
        options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor(
            (value, name) => $"Pole {name} może zawierać tylko cyfry");

        options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(
            (name) => $"Pole {name} może zawierać tylko cyfry");
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// KOLEJNOŚĆ MIDDLEWARE
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();