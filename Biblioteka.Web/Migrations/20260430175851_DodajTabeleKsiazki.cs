using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteka.Web.Migrations
{
    /// <inheritdoc />
    public partial class DodajTabeleKsiazki : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ksiazki",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Tytul = table.Column<string>(type: "TEXT", nullable: false),
                    Autorzy = table.Column<string>(type: "TEXT", nullable: false),
                    Gatunek = table.Column<string>(type: "TEXT", nullable: false),
                    liczba_stron = table.Column<int>(type: "INTEGER", nullable: false),
                    Wydawnictwo = table.Column<string>(type: "TEXT", nullable: false),
                    rok_wydania = table.Column<int>(type: "INTEGER", nullable: false),
                    Cena = table.Column<decimal>(type: "TEXT", nullable: false),
                    liczba_sztuk = table.Column<int>(type: "INTEGER", nullable: false),
                    Opis = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ksiazki", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ksiazki");
        }
    }
}
