using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class ActualizareValoriRol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Actualizăm valorile existente din `0` și `1` în 'User' și 'Admin'
            migrationBuilder.Sql("UPDATE Users SET Rol = 'User' WHERE Rol = '0'");
            migrationBuilder.Sql("UPDATE Users SET Rol = 'Admin' WHERE Rol = '1'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revenim valorile din 'User' și 'Admin' la `0` și `1`
            migrationBuilder.Sql("UPDATE Users SET Rol = '0' WHERE Rol = 'User'");
            migrationBuilder.Sql("UPDATE Users SET Rol = '1' WHERE Rol = 'Admin'");
        }
    }
}
