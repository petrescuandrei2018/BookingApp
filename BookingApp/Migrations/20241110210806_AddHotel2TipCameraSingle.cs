using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddHotel2TipCameraSingle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TipCamere",
                columns: new[] { "TipCameraId", "CapacitatePersoane", "CreatedBy", "CreatedDate", "HotelId", "LastModifiedBy", "LastModifiedDate", "Name", "NrCamereDisponibile", "NrCamereOcupate", "NrTotalCamere" },
                values: new object[] { 6, 1, null, null, 2, null, null, "Single", 5, 5, 10 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TipCamere",
                keyColumn: "TipCameraId",
                keyValue: 6);
        }
    }
}
