using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteka.Web.Migrations
{
    /// <inheritdoc />
    public partial class SeedMissingRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Uprawnienia",
                columns: new[] { "Id", "Nazwa", "Opis" },
                values: new object[] { 4, "Manager", "Zarządzanie treściami i użytkownikami" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Uprawnienia",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
