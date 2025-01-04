using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class ActualizeazaValoriRol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Convertim coloana "Rol" din `int` în `nvarchar(50)`
            migrationBuilder.AlterColumn<string>(
                name: "Rol",
                table: "Users",
                type: "nvarchar(50)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            // Actualizăm valorile existente
            migrationBuilder.Sql("UPDATE Users SET Rol = 'User' WHERE Rol = '0'");
            migrationBuilder.Sql("UPDATE Users SET Rol = 'Admin' WHERE Rol = '1'");

            // Setăm valoarea implicită pentru utilizatorii noi la 'User'
            migrationBuilder.AlterColumn<string>(
                name: "Rol",
                table: "Users",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "User",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revertim coloana "Rol" la `int`
            migrationBuilder.AlterColumn<int>(
                name: "Rol",
                table: "Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)");

            // Revertim valorile text la valori numerice
            migrationBuilder.Sql("UPDATE Users SET Rol = 0 WHERE Rol = 'User'");
            migrationBuilder.Sql("UPDATE Users SET Rol = 1 WHERE Rol = 'Admin'");
        }
    }
}
