using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteka.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddRegistrationDataToKsiazka : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "rok_wydania",
                table: "Ksiazki",
                newName: "RokWydania");

            migrationBuilder.RenameColumn(
                name: "liczba_sztuk",
                table: "Ksiazki",
                newName: "LiczbaSztuk");

            migrationBuilder.RenameColumn(
                name: "liczba_stron",
                table: "Ksiazki",
                newName: "LiczbaStron");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataRejestracji",
                table: "Ksiazki",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "OsobaRejestrujaca",
                table: "Ksiazki",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataRejestracji",
                table: "Ksiazki");

            migrationBuilder.DropColumn(
                name: "OsobaRejestrujaca",
                table: "Ksiazki");

            migrationBuilder.RenameColumn(
                name: "RokWydania",
                table: "Ksiazki",
                newName: "rok_wydania");

            migrationBuilder.RenameColumn(
                name: "LiczbaSztuk",
                table: "Ksiazki",
                newName: "liczba_sztuk");

            migrationBuilder.RenameColumn(
                name: "LiczbaStron",
                table: "Ksiazki",
                newName: "liczba_stron");
        }
    }
}
