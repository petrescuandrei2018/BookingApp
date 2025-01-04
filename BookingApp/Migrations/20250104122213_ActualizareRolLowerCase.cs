using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class ActualizareRolLowerCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Convertim coloana "Rol" în format text și setăm collation
            migrationBuilder.AlterColumn<string>(
                name: "Rol",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "user", // implicit user, litere mici
                collation: "SQL_Latin1_General_CP1_CI_AS",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Actualizăm valorile existente pentru a fi în litere mici
            migrationBuilder.Sql("UPDATE Users SET Rol = LOWER(Rol)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revertim modificările
            migrationBuilder.AlterColumn<string>(
                name: "Rol",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldCollation: "SQL_Latin1_General_CP1_CI_AS");
        }
    }
}
