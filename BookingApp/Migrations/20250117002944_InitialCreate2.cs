using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$nnMblqqzY6fYmFfMptwIcuzrObU/5XN/DdkBbavUovooOlRp7akRS");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$Ns7hbbbPU71guWod932J2uB3/9VUFYsoxW1P2P6hXFlTTAAH1c..m");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "Password",
                value: "$2a$11$ZAJDuH43A00EtWAbslv17ug5pGd7z8nHVBePzToIwcom7Ze3s7S.a");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$dUEvknqvCQRoO0QoxpCRCeR.3HNBIqnL81Ntys5QTm1LqlunYOffy");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$kZawrOlDiOnsaapcMlXreuG4iGmWgaOv9qPNXK4INkIXcU9PB6Tce");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "Password",
                value: "$2a$11$xRb2GPtzBHxxAXxSDH/Z0.wYgK8uIUachK.E/FZ0Z2P3vxjsVivd.");
        }
    }
}
