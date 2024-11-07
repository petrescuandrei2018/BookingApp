using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTipCameraSeaView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TipCamere",
                keyColumn: "TipCameraId",
                keyValue: 10);

            migrationBuilder.InsertData(
                table: "TipCamere",
                columns: new[] { "TipCameraId", "CapacitatePersoane", "CreatedBy", "CreatedDate", "HotelId", "LastModifiedBy", "LastModifiedDate", "Name", "NrCamereDisponibile", "NrCamereOcupate", "NrTotalCamere" },
                values: new object[] { 4, 2, null, null, 2, null, null, "SeaView", 2, 13, 15 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TipCamere",
                keyColumn: "TipCameraId",
                keyValue: 4);

            migrationBuilder.InsertData(
                table: "TipCamere",
                columns: new[] { "TipCameraId", "CapacitatePersoane", "CreatedBy", "CreatedDate", "HotelId", "LastModifiedBy", "LastModifiedDate", "Name", "NrCamereDisponibile", "NrCamereOcupate", "NrTotalCamere" },
                values: new object[] { 10, 2, null, null, 2, null, null, "SeaView", 2, 13, 15 });
        }
    }
}
