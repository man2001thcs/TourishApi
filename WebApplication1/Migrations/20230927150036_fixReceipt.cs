using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class fixReceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "totalNumber",
                table: "FullReceipt",
                newName: "TotalNumber");

            migrationBuilder.RenameColumn(
                name: "singlePrice",
                table: "FullReceipt",
                newName: "SinglePrice");

            migrationBuilder.RenameColumn(
                name: "discountFloat",
                table: "FullReceipt",
                newName: "DiscountFloat");

            migrationBuilder.RenameColumn(
                name: "discountAmount",
                table: "FullReceipt",
                newName: "DiscountAmount");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Receipt",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "Receipt",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Receipt");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "Receipt");

            migrationBuilder.RenameColumn(
                name: "TotalNumber",
                table: "FullReceipt",
                newName: "totalNumber");

            migrationBuilder.RenameColumn(
                name: "SinglePrice",
                table: "FullReceipt",
                newName: "singlePrice");

            migrationBuilder.RenameColumn(
                name: "DiscountFloat",
                table: "FullReceipt",
                newName: "discountFloat");

            migrationBuilder.RenameColumn(
                name: "DiscountAmount",
                table: "FullReceipt",
                newName: "discountAmount");
        }
    }
}
