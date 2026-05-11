using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteka.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Wypozyczenia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KlientId = table.Column<int>(type: "INTEGER", nullable: false),
                    BibliotekarzId = table.Column<string>(type: "TEXT", nullable: false),
                    DataWypozyczenia = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TerminZwrotu = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wypozyczenia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wypozyczenia_Pozycje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WypozyczenieId = table.Column<int>(type: "INTEGER", nullable: false),
                    KsiazkaId = table.Column<int>(type: "INTEGER", nullable: false),
                    DataFaktycznegoZwrotu = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wypozyczenia_Pozycje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wypozyczenia_Pozycje_Ksiazki_KsiazkaId",
                        column: x => x.KsiazkaId,
                        principalTable: "Ksiazki",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Wypozyczenia_Pozycje_Wypozyczenia_WypozyczenieId",
                        column: x => x.WypozyczenieId,
                        principalTable: "Wypozyczenia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wypozyczenia_Pozycje_KsiazkaId",
                table: "Wypozyczenia_Pozycje",
                column: "KsiazkaId");

            migrationBuilder.CreateIndex(
                name: "IX_Wypozyczenia_Pozycje_WypozyczenieId",
                table: "Wypozyczenia_Pozycje",
                column: "WypozyczenieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Wypozyczenia_Pozycje");

            migrationBuilder.DropTable(
                name: "Wypozyczenia");
        }
    }
}
