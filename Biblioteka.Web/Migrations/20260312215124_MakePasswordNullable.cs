using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteka.Web.Migrations
{
    /// <inheritdoc />
    public partial class MakePasswordNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Uzytkownicy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Login = table.Column<string>(type: "TEXT", nullable: false),
                    haslo_hash = table.Column<string>(type: "TEXT", nullable: false),
                    Imie = table.Column<string>(type: "TEXT", nullable: false),
                    Nazwisko = table.Column<string>(type: "TEXT", nullable: false),
                    Pesel = table.Column<string>(type: "TEXT", nullable: false),
                    data_urodzenia = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Plec = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Telefon = table.Column<string>(type: "TEXT", nullable: false),
                    Miejscowosc = table.Column<string>(type: "TEXT", nullable: false),
                    kod_pocztowy = table.Column<string>(type: "TEXT", nullable: false),
                    Ulica = table.Column<string>(type: "TEXT", nullable: false),
                    numer_posesji = table.Column<string>(type: "TEXT", nullable: false),
                    NumerLokalu = table.Column<string>(type: "TEXT", nullable: false),
                    CzyZablokowany = table.Column<bool>(type: "INTEGER", nullable: false),
                    blokada_do = table.Column<DateTime>(type: "TEXT", nullable: true),
                    liczba_blednych_logowan = table.Column<int>(type: "INTEGER", nullable: false),
                    czy_zapomniany = table.Column<bool>(type: "INTEGER", nullable: false),
                    data_zapomnienia = table.Column<DateTime>(type: "TEXT", nullable: true),
                    zapomniany_przez_id = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uzytkownicy", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Uzytkownicy");
        }
    }
}
