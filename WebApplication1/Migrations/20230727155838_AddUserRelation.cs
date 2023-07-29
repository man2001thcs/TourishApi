using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Category",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Receipt_UserId",
                table: "Receipt",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Receipt",
                table: "Receipt",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Receipt",
                table: "Receipt");

            migrationBuilder.DropIndex(
                name: "IX_Receipt_UserId",
                table: "Receipt");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Category");
        }
    }
}
