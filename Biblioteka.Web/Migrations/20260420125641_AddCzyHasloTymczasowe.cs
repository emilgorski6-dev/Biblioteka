using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteka.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddCzyHasloTymczasowe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "czy_haslo_tymczasowe",
                table: "Uzytkownicy",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "czy_haslo_tymczasowe",
                table: "Uzytkownicy");
        }
    }
}
