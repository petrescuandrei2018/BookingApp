using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class AdaugaRecenziiAvansate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Recenzii",
                columns: table => new
                {
                    RecenzieId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rating = table.Column<decimal>(type: "decimal(2,1)", nullable: false),
                    Descriere = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AspectePozitive = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AspecteNegative = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecomandaHotelul = table.Column<bool>(type: "bit", nullable: false),
                    DataRecenziei = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UtilizatorId = table.Column<int>(type: "int", nullable: false),
                    HotelId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
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

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$30u7YqWhdmyp0nWC3LaMHeCMGO0.B7hm.BWhrghXHVfP225BoEkvu");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$valVxen2fq0eKUcB3eTgouRcHnnkkAYdsoqk4kvlBLSPRhrsaaDom");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "Password",
                value: "$2a$11$g1IG8EYf09vW/Qsg9SbyceIcBwlPI8QwHhvBekRDHmwI5odQ1SsGK");

            migrationBuilder.CreateIndex(
                name: "IX_Recenzii_HotelId",
                table: "Recenzii",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Recenzii_UtilizatorId",
                table: "Recenzii",
                column: "UtilizatorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Recenzii");

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
    }
}
