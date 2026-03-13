using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteka.Web.Migrations
{
    /// <inheritdoc />
    public partial class FullSyncWithDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Uprawnienia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nazwa = table.Column<string>(type: "TEXT", nullable: false),
                    Opis = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uprawnienia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Uzytkownicy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Login = table.Column<string>(type: "TEXT", nullable: false),
                    haslo_hash = table.Column<string>(type: "TEXT", nullable: true),
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
                    czy_zablokowany = table.Column<bool>(type: "INTEGER", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "Historia_Hasel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    uzytkownik_id = table.Column<int>(type: "INTEGER", nullable: false),
                    haslo_hash = table.Column<string>(type: "TEXT", nullable: false),
                    data_nadania = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Historia_Hasel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Historia_Hasel_Uzytkownicy_uzytkownik_id",
                        column: x => x.uzytkownik_id,
                        principalTable: "Uzytkownicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Uzytkownik_Uprawnienia",
                columns: table => new
                {
                    uprawnienie_id = table.Column<int>(type: "INTEGER", nullable: false),
                    uzytkownik_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Uzytkownik_Uprawnienia", x => new { x.uprawnienie_id, x.uzytkownik_id });
                    table.ForeignKey(
                        name: "FK_Uzytkownik_Uprawnienia_Uprawnienia_uprawnienie_id",
                        column: x => x.uprawnienie_id,
                        principalTable: "Uprawnienia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Uzytkownik_Uprawnienia_Uzytkownicy_uzytkownik_id",
                        column: x => x.uzytkownik_id,
                        principalTable: "Uzytkownicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Historia_Hasel_uzytkownik_id",
                table: "Historia_Hasel",
                column: "uzytkownik_id");

            migrationBuilder.CreateIndex(
                name: "IX_Uzytkownik_Uprawnienia_uzytkownik_id",
                table: "Uzytkownik_Uprawnienia",
                column: "uzytkownik_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Historia_Hasel");

            migrationBuilder.DropTable(
                name: "Uzytkownik_Uprawnienia");

            migrationBuilder.DropTable(
                name: "Uprawnienia");

            migrationBuilder.DropTable(
                name: "Uzytkownicy");
        }
    }
}
