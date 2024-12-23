using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingApp.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDuplicateBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Delete duplicate bookings while keeping the first occurrence
            migrationBuilder.Sql(@"
        WITH CTE_Duplicates AS (
            SELECT 
                RezervareId, 
                ROW_NUMBER() OVER (
                    PARTITION BY CheckIn, CheckOut, PretCameraId
                    ORDER BY RezervareId
                ) AS RowNumber
            FROM Rezervari
        )
        DELETE FROM Rezervari
        WHERE RezervareId IN (
            SELECT RezervareId
            FROM CTE_Duplicates
            WHERE RowNumber > 1
        );
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
