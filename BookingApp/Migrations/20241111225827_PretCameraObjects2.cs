using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class PretCameraObjects2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PretCamere",
                columns: new[] { "PretCameraId", "CreatedBy", "CreatedDate", "EndPretCamera", "LastModifiedBy", "LastModifiedDate", "PretNoapte", "StartPretCamera", "TipCameraId" },
                values: new object[,]
                {
                    { 1, null, null, new DateTime(2024, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, 0f, new DateTime(2024, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 2, null, null, new DateTime(2024, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, 0f, new DateTime(2024, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 3, null, null, new DateTime(2024, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, 0f, new DateTime(2024, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 5 },
                    { 4, null, null, new DateTime(2024, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, 0f, new DateTime(2024, 8, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 6 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PretCamere",
                keyColumn: "PretCameraId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PretCamere",
                keyColumn: "PretCameraId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PretCamere",
                keyColumn: "PretCameraId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PretCamere",
                keyColumn: "PretCameraId",
                keyValue: 4);
        }
    }
}
