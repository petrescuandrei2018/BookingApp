using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class AdaugaLatitudineLongitudineHoteluri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitudine",
                table: "Hotels",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitudine",
                table: "Hotels",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "Hotels",
                keyColumn: "HotelId",
                keyValue: 1,
                columns: new[] { "Latitudine", "Longitudine" },
                values: new object[] { 45.653199999999998, 25.6113 });

            migrationBuilder.UpdateData(
                table: "Hotels",
                keyColumn: "HotelId",
                keyValue: 2,
                columns: new[] { "Latitudine", "Longitudine" },
                values: new object[] { 44.159799999999997, 28.634799999999998 });

            migrationBuilder.UpdateData(
                table: "Hotels",
                keyColumn: "HotelId",
                keyValue: 3,
                columns: new[] { "Latitudine", "Longitudine" },
                values: new object[] { 45.798299999999998, 24.125599999999999 });

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "ReviewId",
                keyValue: 2,
                column: "Description",
                value: "Mâncare excelentă");

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "ReviewId",
                keyValue: 4,
                column: "Description",
                value: "Cazare târzie");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$cJPtg.TRjTuAkFLcT2vpSOAIX1nE3sGrlb6icDJ1LTzE2JXnzYUJC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$3Z0LL0J8coizPMG0nWZMzuk3yHSEDYFYJOyCVfbud4PR7QHgP6MMS");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "Password",
                value: "$2a$11$K35XABLTmnHxKt.sZaJ2KeemvTKeNUWceLk9oIAa2ACdfxhKI7GXS");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitudine",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "Longitudine",
                table: "Hotels");

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "ReviewId",
                keyValue: 2,
                column: "Description",
                value: "Mancare excelenta");

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "ReviewId",
                keyValue: 4,
                column: "Description",
                value: "Cazare tarzie");

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
    }
}
