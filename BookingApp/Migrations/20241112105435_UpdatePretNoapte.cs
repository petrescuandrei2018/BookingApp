using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePretNoapte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PretCamere",
                keyColumn: "PretCameraId",
                keyValue: 1,
                column: "PretNoapte",
                value: 900f);

            migrationBuilder.UpdateData(
                table: "PretCamere",
                keyColumn: "PretCameraId",
                keyValue: 2,
                column: "PretNoapte",
                value: 700f);

            migrationBuilder.UpdateData(
                table: "PretCamere",
                keyColumn: "PretCameraId",
                keyValue: 3,
                column: "PretNoapte",
                value: 500f);

            migrationBuilder.UpdateData(
                table: "PretCamere",
                keyColumn: "PretCameraId",
                keyValue: 4,
                column: "PretNoapte",
                value: 550f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PretCamere",
                keyColumn: "PretCameraId",
                keyValue: 1,
                column: "PretNoapte",
                value: 0f);

            migrationBuilder.UpdateData(
                table: "PretCamere",
                keyColumn: "PretCameraId",
                keyValue: 2,
                column: "PretNoapte",
                value: 0f);

            migrationBuilder.UpdateData(
                table: "PretCamere",
                keyColumn: "PretCameraId",
                keyValue: 3,
                column: "PretNoapte",
                value: 0f);

            migrationBuilder.UpdateData(
                table: "PretCamere",
                keyColumn: "PretCameraId",
                keyValue: 4,
                column: "PretNoapte",
                value: 0f);
        }
    }
}
