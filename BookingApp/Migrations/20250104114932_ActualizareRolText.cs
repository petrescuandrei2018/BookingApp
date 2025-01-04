using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class ActualizareRolText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Convertim coloana "Rol" din `int` în `nvarchar(max)`
            migrationBuilder.AlterColumn<string>(
                name: "Rol",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "User",
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reconvertim coloana "Rol" în `int`
            migrationBuilder.AlterColumn<int>(
                name: "Rol",
                table: "Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Conversia valorilor text existente în numerice
            migrationBuilder.Sql("UPDATE Users SET Rol = 0 WHERE Rol = 'User'");
            migrationBuilder.Sql("UPDATE Users SET Rol = 1 WHERE Rol = 'Admin'");
        }
    }
}