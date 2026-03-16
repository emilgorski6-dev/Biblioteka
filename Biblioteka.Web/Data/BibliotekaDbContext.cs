using Microsoft.EntityFrameworkCore;
using Biblioteka.Web.Data.Entities;
using System;
using System.Collections.Generic;

namespace Biblioteka.Web.Data
{
    public class BibliotekaDbContext : DbContext
    {
        public BibliotekaDbContext(DbContextOptions<BibliotekaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Uzytkownik> Uzytkownicy { get; set; }
        public DbSet<Uprawnienie> Uprawnienia { get; set; }
        public DbSet<HistoriaHasla> HistoriaHasel { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Uzytkownik>()
                .HasMany(u => u.Uprawnienia)
                .WithMany(p => p.Uzytkownicy)
                .UsingEntity<Dictionary<string, object>>(
                    "Uzytkownik_Uprawnienia",
                    j => j.HasOne<Uprawnienie>().WithMany().HasForeignKey("uprawnienie_id"),
                    j => j.HasOne<Uzytkownik>().WithMany().HasForeignKey("uzytkownik_id"));

            modelBuilder.Entity<Uprawnienie>().HasData(
                new Uprawnienie { Id = 1, Nazwa = "Administrator", Opis = "Pełny dostęp do systemu" },
                new Uprawnienie { Id = 2, Nazwa = "Bibliotekarz", Opis = "Zarządzanie książkami i wypożyczeniami" },
                new Uprawnienie { Id = 3, Nazwa = "Klient", Opis = "Podstawowy dostęp dla czytelników" }
            );
            modelBuilder.Entity<Uzytkownik>().HasData(
                new Uzytkownik
                {
                    Id = 1,
                    Login = "admin",
                    Imie = "Emil",
                    Nazwisko = "Górski",
                    Email = "admin@biblioteka.pl",
                    Pesel = "90010112345",
                    DataUrodzenia = new DateTime(1990, 1, 1),
                    Plec = "mężczyzna",
                    Telefon = "123456789",
                    Miejscowosc = "Łódź",
                    KodPocztowy = "90-001",
                    Ulica = "Piotrkowska",
                    NumerPosesji = "1",
                    NumerLokalu = "",
                    HasloHash = "admin123",
                    CzyZablokowany = false,
                    LiczbaBlednychLogowan = 0,
                    CzyZapomniany = false
                }
            );
        }
    }
}