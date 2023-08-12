using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class updateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "discountFloat",
                table: "Voucher",
                newName: "DiscountFloat");

            migrationBuilder.RenameColumn(
                name: "discountAmount",
                table: "Voucher",
                newName: "DiscountAmount");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Voucher",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Voucher");

            migrationBuilder.RenameColumn(
                name: "DiscountFloat",
                table: "Voucher",
                newName: "discountFloat");

            migrationBuilder.RenameColumn(
                name: "DiscountAmount",
                table: "Voucher",
                newName: "discountAmount");
        }
    }
}
