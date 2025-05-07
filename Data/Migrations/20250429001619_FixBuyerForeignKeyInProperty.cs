using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaharlikaGrandEstate.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixBuyerForeignKeyInProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_AspNetUsers_BuyerId",
                table: "Properties");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_AspNetUsers_BuyerId",
                table: "Properties",
                column: "BuyerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_AspNetUsers_BuyerId",
                table: "Properties");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_AspNetUsers_BuyerId",
                table: "Properties",
                column: "BuyerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
