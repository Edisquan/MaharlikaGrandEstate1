using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaharlikaGrandEstate.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationFeeToProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ReservationFee",
                table: "Properties",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m // Default to 0 to avoid errors
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReservationFee",
                table: "Properties"
            );
        }
    }
}
