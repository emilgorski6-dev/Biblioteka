using Biblioteka.Web.Data;
using Biblioteka.Web.Services; // Dodane
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BibliotekaDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// REJESTRACJA SERWISÓW (Ważne dla działania kontrolerów)
builder.Services.AddScoped<PasswordService>();

// EMAIL SENDER
builder.Services.AddScoped<IEmailService, EmailService>();

// WBUDOWANA OBSŁUGA LOGOWANIA/WYLOGOWANIA
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// KOLEJNOŚĆ MIDDLEWARE (Kluczowa dla bezpieczeństwa)
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();