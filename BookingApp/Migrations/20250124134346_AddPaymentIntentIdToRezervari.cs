using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentIntentIdToRezervari : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "PretNoapte",
                table: "PretCamere",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

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

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$8ulyu2PWug.7QpzvRyiSdeS55zZGVxk95XBAjRLaN1LkjDlykQRVK");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$Y7xbXs3KN2p2w/n2rZjj3u9KBgit6/7zinZKfZnB/kACJvLOcu45y");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "Password",
                value: "$2a$11$WPPOlwb3XhhjJA8R0135Ge5ymPJ9/kLMrm3Jgrr5tz1BVf50zafTO");

            migrationBuilder.AddColumn<string>(
        name: "PaymentIntentId",
        table: "Rezervari",
        type: "nvarchar(255)",
        nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PretNoapte",
                table: "PretCamere",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.UpdateData(
                table: "PretCamere",
                keyColumn: "PretCameraId",
                keyValue: 1,
                column: "PretNoapte",
                value: 900m);

            migrationBuilder.DropColumn(
        name: "PaymentIntentId",
        table: "Rezervari");

            migrationBuilder.UpdateData(
                table: "PretCamere",
                keyColumn: "PretCameraId",
                keyValue: 2,
                column: "PretNoapte",
                value: 700m);

            migrationBuilder.UpdateData(
                table: "PretCamere",
                keyColumn: "PretCameraId",
                keyValue: 3,
                column: "PretNoapte",
                value: 500m);

            migrationBuilder.UpdateData(
                table: "PretCamere",
                keyColumn: "PretCameraId",
                keyValue: 4,
                column: "PretNoapte",
                value: 550m);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$/0EbQOyhBzORPgoxn2JTIOBq..URwAkw0M2mR9dZeH.XlMdjmvwYe");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$vYWL88.CtJdPn5Fis0d5mu8SifUEGcXeJcPGhLAc9XQYJGMwXLLQe");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "Password",
                value: "$2a$11$/nM33WuKquGzrFqWRfqPvOVYBTQ1d/wPvz60xkeGgcgQkf6om6dFC");
        }
    }
}
