using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class deleteContraintSaveFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaveFile_Message",
                table: "SaveFile");

            migrationBuilder.DropIndex(
                name: "IX_SaveFile_AccessSourceId",
                table: "SaveFile");

            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "SaveFile");

            migrationBuilder.AddColumn<Guid>(
                name: "Bookid",
                table: "SaveFile",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MessageId",
                table: "SaveFile",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaveFile_Bookid",
                table: "SaveFile",
                column: "Bookid");

            migrationBuilder.CreateIndex(
                name: "IX_SaveFile_MessageId",
                table: "SaveFile",
                column: "MessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaveFile_Book_Bookid",
                table: "SaveFile",
                column: "Bookid",
                principalTable: "Book",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_SaveFile_Message_MessageId",
                table: "SaveFile",
                column: "MessageId",
                principalTable: "Message",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaveFile_Book_Bookid",
                table: "SaveFile");

            migrationBuilder.DropForeignKey(
                name: "FK_SaveFile_Message_MessageId",
                table: "SaveFile");

            migrationBuilder.DropIndex(
                name: "IX_SaveFile_Bookid",
                table: "SaveFile");

            migrationBuilder.DropIndex(
                name: "IX_SaveFile_MessageId",
                table: "SaveFile");

            migrationBuilder.DropColumn(
                name: "Bookid",
                table: "SaveFile");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "SaveFile");

            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "SaveFile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SaveFile_AccessSourceId",
                table: "SaveFile",
                column: "AccessSourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaveFile_Message",
                table: "SaveFile",
                column: "AccessSourceId",
                principalTable: "Message",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
