using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaharlikaGrandEstate.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddArchiveField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Properties",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Properties");
        }
    }
}
