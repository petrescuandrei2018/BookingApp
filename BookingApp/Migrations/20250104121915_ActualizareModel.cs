using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class ActualizareModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Rol",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "user",
                collation: "SQL_Latin1_General_CP1_CI_AS",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "User");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Rol",
                value: "user");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Rol",
                value: "user");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "Rol",
                value: "user");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Rol",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "User",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "user",
                oldCollation: "SQL_Latin1_General_CP1_CI_AS");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Rol",
                value: "User");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Rol",
                value: "User");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "Rol",
                value: "User");
        }
    }
}
