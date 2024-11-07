using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTipCameraApartament : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TipCamere",
                columns: new[] { "TipCameraId", "CapacitatePersoane", "CreatedBy", "CreatedDate", "HotelId", "LastModifiedBy", "LastModifiedDate", "Name", "NrCamereDisponibile", "NrCamereOcupate", "NrTotalCamere" },
                values: new object[] { 3, 4, null, null, 1, null, null, "Apartament", 1, 9, 10 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TipCamere",
                keyColumn: "TipCameraId",
                keyValue: 3);
        }
    }
}
