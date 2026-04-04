using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteka.Web.Migrations
{
    /// <inheritdoc />
    public partial class PoprawaStrukturyUzytkownika : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Ulica",
                table: "Uzytkownicy",
                newName: "ulica");

            migrationBuilder.RenameColumn(
                name: "Telefon",
                table: "Uzytkownicy",
                newName: "telefon");

            migrationBuilder.RenameColumn(
                name: "Plec",
                table: "Uzytkownicy",
                newName: "plec");

            migrationBuilder.RenameColumn(
                name: "Pesel",
                table: "Uzytkownicy",
                newName: "pesel");

            migrationBuilder.RenameColumn(
                name: "Nazwisko",
                table: "Uzytkownicy",
                newName: "nazwisko");

            migrationBuilder.RenameColumn(
                name: "Miejscowosc",
                table: "Uzytkownicy",
                newName: "miejscowosc");

            migrationBuilder.RenameColumn(
                name: "Login",
                table: "Uzytkownicy",
                newName: "login");

            migrationBuilder.RenameColumn(
                name: "Imie",
                table: "Uzytkownicy",
                newName: "imie");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Uzytkownicy",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Uzytkownicy",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "NumerLokalu",
                table: "Uzytkownicy",
                newName: "numer_lokalu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ulica",
                table: "Uzytkownicy",
                newName: "Ulica");

            migrationBuilder.RenameColumn(
                name: "telefon",
                table: "Uzytkownicy",
                newName: "Telefon");

            migrationBuilder.RenameColumn(
                name: "plec",
                table: "Uzytkownicy",
                newName: "Plec");

            migrationBuilder.RenameColumn(
                name: "pesel",
                table: "Uzytkownicy",
                newName: "Pesel");

            migrationBuilder.RenameColumn(
                name: "nazwisko",
                table: "Uzytkownicy",
                newName: "Nazwisko");

            migrationBuilder.RenameColumn(
                name: "miejscowosc",
                table: "Uzytkownicy",
                newName: "Miejscowosc");

            migrationBuilder.RenameColumn(
                name: "login",
                table: "Uzytkownicy",
                newName: "Login");

            migrationBuilder.RenameColumn(
                name: "imie",
                table: "Uzytkownicy",
                newName: "Imie");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Uzytkownicy",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Uzytkownicy",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "numer_lokalu",
                table: "Uzytkownicy",
                newName: "NumerLokalu");
        }
    }
}
