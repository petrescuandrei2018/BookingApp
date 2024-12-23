using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddStareRezervare : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Rezervari");

            migrationBuilder.AddColumn<string>(
                name: "Stare",
                table: "Rezervari",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stare",
                table: "Rezervari");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Rezervari",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Active");
        }
    }
}
