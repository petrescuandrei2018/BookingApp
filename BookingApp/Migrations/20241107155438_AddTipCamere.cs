using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTipCamere : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TipCamere",
                keyColumn: "TipCameraId",
                keyValue: 1,
                column: "HotelId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "TipCamere",
                keyColumn: "TipCameraId",
                keyValue: 2,
                column: "HotelId",
                value: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TipCamere",
                keyColumn: "TipCameraId",
                keyValue: 1,
                column: "HotelId",
                value: 0);

            migrationBuilder.UpdateData(
                table: "TipCamere",
                keyColumn: "TipCameraId",
                keyValue: 2,
                column: "HotelId",
                value: 0);
        }
    }
}
