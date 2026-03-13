using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Biblioteka.Web.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Uprawnienia",
                columns: new[] { "Id", "Nazwa", "Opis" },
                values: new object[,]
                {
                    { 1, "Administrator", "Pełny dostęp do systemu" },
                    { 2, "Bibliotekarz", "Zarządzanie książkami i wypożyczeniami" },
                    { 3, "Klient", "Podstawowy dostęp dla czytelników" }
                });

            migrationBuilder.InsertData(
                table: "Uzytkownicy",
                columns: new[] { "Id", "blokada_do", "czy_zablokowany", "czy_zapomniany", "data_urodzenia", "data_zapomnienia", "Email", "haslo_hash", "Imie", "kod_pocztowy", "liczba_blednych_logowan", "Login", "Miejscowosc", "Nazwisko", "NumerLokalu", "numer_posesji", "Pesel", "Plec", "Telefon", "Ulica", "zapomniany_przez_id" },
                values: new object[] { 1, null, false, false, new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "admin@biblioteka.pl", "admin123", "Emil", "90-001", 0, "admin", "Łódź", "Górski", "", "1", "90010112345", "mężczyzna", "123456789", "Piotrkowska", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Uprawnienia",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Uprawnienia",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Uprawnienia",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Uzytkownicy",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
