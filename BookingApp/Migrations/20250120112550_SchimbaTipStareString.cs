using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class SchimbaTipStareString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Stare",
                table: "Rezervari",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$hWoUffwUT9McLkz6FbnG..pp5Llz.4YgnSFuorrOxoXPvsPCxBE3C");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$HZsXYUuvF3caE30wrqxNYeLNWDkx79Vp6gVSVBavAb0ern1We9VCm");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "Password",
                value: "$2a$11$tkJjjHsNH3ya5wp6Xb1UM.0fZF4B5Xk245Xlzxh2T.EsVrSE5fFnu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Stare",
                table: "Rezervari",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$OUs95r6D0E76ubMnuJWJC.WCQdkbyke9au7rDPAnEz8SNR7Wah8w.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$mRaTjfq1hngFobjB1flQi.MGx6YZ4s7MB0ro74mTIOk6wVI2SNq.C");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "Password",
                value: "$2a$11$.LfBmFSZXnT//FpuJaETI.eDGoGFUZGpOBcu1h3od.3b4tNYuqxyW");
        }
    }
}
