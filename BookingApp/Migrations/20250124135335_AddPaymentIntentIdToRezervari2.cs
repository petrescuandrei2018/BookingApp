using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentIntentIdToRezervari2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Adaugă coloana PaymentIntentId
            migrationBuilder.AddColumn<string>(
                name: "PaymentIntentId",
                table: "Rezervari",
                type: "nvarchar(255)", // Specifică tipul coloanei
                maxLength: 255,        // Lungime maximă
                nullable: true);       // Permite valori NULL

            // Alte update-uri (opțional)
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$7.SPtZaFFjHfYw.9uFqT1eKNaPV9aVJHbdmomwXQxmYlyG9jTTuDa");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$vp4CmMGk55PNL.vtfv7QzO9n0ulrlJT.N9vWVJxjpsgjtbahy7cQC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "Password",
                value: "$2a$11$NoMBuMeRH1oqEX3aFGzjeOogSxdVBS.uFnKWCZnxn8SNmYPCquToW");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Elimină coloana PaymentIntentId
            migrationBuilder.DropColumn(
                name: "PaymentIntentId",
                table: "Rezervari");

            // Alte rollback-uri (opțional)
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
        }
    }
}
