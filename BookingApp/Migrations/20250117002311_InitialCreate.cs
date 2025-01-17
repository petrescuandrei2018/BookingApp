using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hotels",
                columns: table => new
                {
                    HotelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotels", x => x.HotelId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Varsta = table.Column<int>(type: "int", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "user")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "TipCamere",
                columns: table => new
                {
                    TipCameraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CapacitatePersoane = table.Column<int>(type: "int", nullable: false),
                    NrTotalCamere = table.Column<int>(type: "int", nullable: false),
                    NrCamereDisponibile = table.Column<int>(type: "int", nullable: false),
                    NrCamereOcupate = table.Column<int>(type: "int", nullable: false),
                    HotelId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipCamere", x => x.TipCameraId);
                    table.ForeignKey(
                        name: "FK_TipCamere_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "HotelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HotelId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Rezervari",
                columns: table => new
                {
                    RezervareId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PretCameraId = table.Column<int>(type: "int", nullable: false),
                    CheckIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOut = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Stare = table.Column<int>(type: "int", nullable: false),
                    StarePlata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientSecret = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SumaTotala = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SumaAchitata = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SumaRamasaDePlata = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezervari", x => x.RezervareId);
                    table.ForeignKey(
                        name: "FK_Rezervari_PretCamere_PretCameraId",
                        column: x => x.PretCameraId,
                        principalTable: "PretCamere",
                        principalColumn: "PretCameraId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rezervari_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "HotelId", "Address", "CreatedBy", "CreatedDate", "LastModifiedBy", "LastModifiedDate", "Name" },
                values: new object[,]
                {
                    { 1, "Brasov", null, null, null, null, "Hotel1" },
                    { 2, "Constanta", null, null, null, null, "Hotel2" },
                    { 3, "Sibu", null, null, null, null, "Hotel3" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "Password", "PhoneNumber", "Rol", "UserName", "Varsta" },
                values: new object[,]
                {
                    { 1, "mihai@gmail.com", "$2a$11$dUEvknqvCQRoO0QoxpCRCeR.3HNBIqnL81Ntys5QTm1LqlunYOffy", "0775695878", "admin", "Mihai", 30 },
                    { 2, "nicu@gmail.com", "$2a$11$kZawrOlDiOnsaapcMlXreuG4iGmWgaOv9qPNXK4INkIXcU9PB6Tce", "0770605078", "admin", "Nicu", 20 },
                    { 3, "alex@gmail.com", "$2a$11$xRb2GPtzBHxxAXxSDH/Z0.wYgK8uIUachK.E/FZ0Z2P3vxjsVivd.", "0765665668", "user", "Alex", 32 }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "ReviewId", "Description", "HotelId", "Rating", "UserId" },
                values: new object[,]
                {
                    { 1, "A fost bine", 1, 4.9000000000000004, 1 },
                    { 2, "Mancare excelenta", 1, 5.0, 2 },
                    { 3, "Priveliste la mare", 1, 5.0, 3 },
                    { 4, "Cazare tarzie", 1, 3.5, 3 },
                    { 5, "Muste in camera", 2, 2.0, 2 }
                });

            migrationBuilder.InsertData(
                table: "TipCamere",
                columns: new[] { "TipCameraId", "CapacitatePersoane", "CreatedBy", "CreatedDate", "HotelId", "LastModifiedBy", "LastModifiedDate", "Name", "NrCamereDisponibile", "NrCamereOcupate", "NrTotalCamere" },
                values: new object[,]
                {
                    { 3, 4, null, null, 1, null, null, "Apartament", 4, 6, 10 },
                    { 4, 2, null, null, 2, null, null, "SeaView", 0, 15, 15 },
                    { 5, 1, null, null, 1, null, null, "Single", 4, 16, 20 },
                    { 6, 1, null, null, 2, null, null, "Single", 0, 10, 10 }
                });

            migrationBuilder.InsertData(
                table: "PretCamere",
                columns: new[] { "PretCameraId", "CreatedBy", "CreatedDate", "EndPretCamera", "LastModifiedBy", "LastModifiedDate", "PretNoapte", "StartPretCamera", "TipCameraId" },
                values: new object[,]
                {
                    { 1, null, null, new DateTime(2024, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, 900f, new DateTime(2024, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 2, null, null, new DateTime(2024, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, 700f, new DateTime(2024, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 3, null, null, new DateTime(2024, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, 500f, new DateTime(2024, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 5 },
                    { 4, null, null, new DateTime(2024, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, 550f, new DateTime(2024, 8, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), 6 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PretCamere_TipCameraId",
                table: "PretCamere",
                column: "TipCameraId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_HotelId",
                table: "Reviews",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervari_PretCameraId",
                table: "Rezervari",
                column: "PretCameraId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervari_UserId",
                table: "Rezervari",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TipCamere_HotelId",
                table: "TipCamere",
                column: "HotelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Rezervari");

            migrationBuilder.DropTable(
                name: "PretCamere");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "TipCamere");

            migrationBuilder.DropTable(
                name: "Hotels");
        }
    }
}
