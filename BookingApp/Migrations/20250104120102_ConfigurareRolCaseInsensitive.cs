using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class ConfigurareRolCaseInsensitive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Rol",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "User",
                collation: "SQL_Latin1_General_CP1_CI_AS",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Update datele existente la forma corectă și case-insensitive
            migrationBuilder.Sql("UPDATE Users SET Rol = LOWER(Rol)");
        }

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
                oldCollation: "SQL_Latin1_General_CP1_CI_AS");
        }
    }
}
