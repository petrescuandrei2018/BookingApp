using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class ReviewsAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "ReviewId", "Description", "HotelId", "Rating", "UserId" },
                values: new object[,]
                {
                    { 1, "A fost bine", 1, 4.9000000000000004, 1 },
                    { 2, "Mancare excelenta", 1, 5.0, 2 },
                    { 3, "Priveliste la mare", 1, 5.0, 3 },
                    { 4, "Cazare tarzie", 1, 3.5, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "ReviewId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "ReviewId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "ReviewId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "ReviewId",
                keyValue: 4);
        }
    }
}
