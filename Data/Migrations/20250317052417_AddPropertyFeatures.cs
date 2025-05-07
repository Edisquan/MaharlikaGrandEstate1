using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaharlikaGrandEstate.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Bathrooms",
                table: "Properties",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Bedrooms",
                table: "Properties",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FurnishingStatus",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "HasParking",
                table: "Properties",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Size",
                table: "Properties",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearBuilt",
                table: "Properties",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bathrooms",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "Bedrooms",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "FurnishingStatus",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "HasParking",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "YearBuilt",
                table: "Properties");
        }
    }
}
