using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaharlikaGrandEstate.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BuyerId",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsSold",
                table: "Properties",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyerId",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "IsSold",
                table: "Properties");
        }
    }
}
