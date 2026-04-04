using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteka.Web.Migrations
{
    /// <inheritdoc />
    public partial class ZmianaPlciNaEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Plec",
                table: "Uzytkownicy",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.UpdateData(
                table: "Uzytkownicy",
                keyColumn: "Id",
                keyValue: 1,
                column: "Plec",
                value: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Plec",
                table: "Uzytkownicy",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.UpdateData(
                table: "Uzytkownicy",
                keyColumn: "Id",
                keyValue: 1,
                column: "Plec",
                value: "mężczyzna");
        }
    }
}
