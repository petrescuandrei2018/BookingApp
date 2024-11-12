using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class PretCamera : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PretCamere",
                columns: table => new
                {
                    PretCameraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PretNoapte = table.Column<float>(type: "real", nullable: false),
                    StartPretCamera = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndPretCamera = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipCameraId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PretCamere", x => x.PretCameraId);
                    table.ForeignKey(
                        name: "FK_PretCamere_TipCamere_TipCameraId",
                        column: x => x.TipCameraId,
                        principalTable: "TipCamere",
                        principalColumn: "TipCameraId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PretCamere_TipCameraId",
                table: "PretCamere",
                column: "TipCameraId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PretCamere");
        }
    }
}
