using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class TestRezolvareRutaGetRezervariNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$mcY1RoyXQYY7.By.2Of1puHXd7KkX9ZlyP4xAwnRX8lVGEufNIPAC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$8vZKI0XPKGEMFx6LNY.38egXJ/c3V6hKdcXaw.Gzioh1AH2OGleqO");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "Password",
                value: "$2a$11$26wzsjr/cPx1oD7HO4X3tuZ/9NZe4oh1meOkzVWTEuS0I.p84slV.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
