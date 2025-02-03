using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class ActualizareRecenzii : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ștergem tabela veche Reviews
            migrationBuilder.DropTable(
                name: "Reviews");

            // Creăm tabela Recenzii
            migrationBuilder.CreateTable(
                name: "Recenzii",
                columns: table => new
                {
                    RecenzieId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    Descriere = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AspectePozitive = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AspecteNegative = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecomandaHotelul = table.Column<bool>(type: "bit", nullable: false),
                    DataRecenziei = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UtilizatorId = table.Column<int>(type: "int", nullable: false),
                    HotelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recenzii", x => x.RecenzieId);
                    table.ForeignKey(
                        name: "FK_Recenzii_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "HotelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Recenzii_Users_UtilizatorId",
                        column: x => x.UtilizatorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            // Seed data pentru Recenzii
            migrationBuilder.InsertData(
                table: "Recenzii",
                columns: new[] { "RecenzieId", "Rating", "Descriere", "AspectePozitive", "AspecteNegative", "RecomandaHotelul", "DataRecenziei", "HotelId", "UtilizatorId" },
                values: new object[,]
                {
            { 1, 4.9, "A fost bine", "Curățenie", "Preț mare", true, DateTime.UtcNow, 1, 1 },
            { 2, 5.0, "Mâncare excelentă", "Bufet variat", null, true, DateTime.UtcNow, 1, 2 },
            { 3, 5.0, "Priveliste la mare", "Locație perfectă", null, true, DateTime.UtcNow, 1, 3 },
            { 4, 3.5, "Cazare târzie", "Personal prietenos", "A durat prea mult check-in-ul", false, DateTime.UtcNow, 3, 1 },
            { 5, 2.0, "Muște în cameră", null, "Curățenie slabă", false, DateTime.UtcNow, 2, 2 }
                });

            // Update parole pentru utilizatori (dacă este necesar)
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$2DoCTB2xda.BNYmvYpOteujba4JNDKiCUKZ2xyYsXItQMRkemn8aS");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$7Ge.C6hkX3I5NowTSpJxJuqSmZeIWQm78OP5TYogzBf/eNi.HCZsm");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "Password",
                value: "$2a$11$g0yF8Xk1vZxOZRD1jlErE./z2ev289.o/SybsDQMbQmCe9ZC0B1/e");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Ștergem tabela Recenzii
            migrationBuilder.DropTable(name: "Recenzii");

            // Re-creăm tabela Reviews în cazul rollback-ului
            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HotelId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_Reviews_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "HotelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_HotelId",
                table: "Reviews",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");
        }
    }
}
