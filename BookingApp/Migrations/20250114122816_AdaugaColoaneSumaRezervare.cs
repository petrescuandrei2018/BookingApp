using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class AdaugaColoaneSumaRezervare : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SumaAchitata",
                table: "Rezervari",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SumaRamasaDePlata",
                table: "Rezervari",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SumaTotala",
                table: "Rezervari",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SumaAchitata",
                table: "Rezervari");

            migrationBuilder.DropColumn(
                name: "SumaRamasaDePlata",
                table: "Rezervari");

            migrationBuilder.DropColumn(
                name: "SumaTotala",
                table: "Rezervari");
        }
    }
}
