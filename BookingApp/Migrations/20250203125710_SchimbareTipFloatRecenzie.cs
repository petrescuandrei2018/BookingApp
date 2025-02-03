using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class SchimbareTipFloatRecenzie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Recenzii");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Recenzii");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Recenzii");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "Recenzii");

            migrationBuilder.AlterColumn<double>(
                name: "Rating",
                table: "Recenzii",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,1)");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$w/s/t4lTngPP0tcdTLoFMuzSbJGrLcgUFMw1DAwNd83w8eNfAyzBe");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Password",
                value: "$2a$11$kY.FASiyHzIPnDRh7WDv3.aPa5oHbkJn7gxUBi5q6xVnZZUuilW26");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "Password",
                value: "$2a$11$Y5M0q0azgMpYqkIAJj5AcedYctCe8FWVJzdJsAcwX9usGcO4gbF8C");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Rating",
                table: "Recenzii",
                type: "decimal(2,1)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Recenzii",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Recenzii",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Recenzii",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Recenzii",
                type: "datetime2",
                nullable: true);

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
    }
}
