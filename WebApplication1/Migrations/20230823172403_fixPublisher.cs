using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class fixPublisher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookPublisher");

            migrationBuilder.CreateIndex(
                name: "IX_Book_PublisherId",
                table: "Book",
                column: "PublisherId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookPublisher_Publisher",
                table: "Book",
                column: "PublisherId",
                principalTable: "Publisher",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookPublisher_Publisher",
                table: "Book");

            migrationBuilder.DropIndex(
                name: "IX_Book_PublisherId",
                table: "Book");

            migrationBuilder.CreateTable(
                name: "BookPublisher",
                columns: table => new
                {
                    BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PublisherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookPublisher", x => new { x.BookId, x.PublisherId });
                    table.ForeignKey(
                        name: "FK_BookPublisher_Book",
                        column: x => x.BookId,
                        principalTable: "Book",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookPublisher_Publisher",
                        column: x => x.PublisherId,
                        principalTable: "Publisher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookPublisher_PublisherId",
                table: "BookPublisher",
                column: "PublisherId");
        }
    }
}
