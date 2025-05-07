using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaharlikaGrandEstate.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInquiryNavProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Inquiries_PropertyId",
                table: "Inquiries",
                column: "PropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inquiries_Properties_PropertyId",
                table: "Inquiries",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inquiries_Properties_PropertyId",
                table: "Inquiries");

            migrationBuilder.DropIndex(
                name: "IX_Inquiries_PropertyId",
                table: "Inquiries");
        }
    }
}
