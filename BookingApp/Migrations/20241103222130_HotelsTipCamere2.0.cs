using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class HotelsTipCamere20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "HotelSiTipCamere",
                columns: new[] { "HotelId", "TipCameraId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 2, 1 },
                    { 2, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "HotelSiTipCamere",
                keyColumns: new[] { "HotelId", "TipCameraId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "HotelSiTipCamere",
                keyColumns: new[] { "HotelId", "TipCameraId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "HotelSiTipCamere",
                keyColumns: new[] { "HotelId", "TipCameraId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "HotelSiTipCamere",
                keyColumns: new[] { "HotelId", "TipCameraId" },
                keyValues: new object[] { 2, 2 });
        }
    }
}
