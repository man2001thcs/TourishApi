using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourishApi.Migrations
{
    /// <inheritdoc />
    public partial class ajustSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduleType",
                table: "TotalReceipt");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "StayingSchedule");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "StayingSchedule");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "StayingSchedule");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "StayingSchedule");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "MovingSchedule");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "MovingSchedule");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "MovingSchedule");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MovingSchedule");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "EatSchedule");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "EatSchedule");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "EatSchedule");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "EatSchedule");

            migrationBuilder.RenameColumn(
                name: "ScheduleId",
                table: "TotalReceipt",
                newName: "StayingScheduleId");

            migrationBuilder.AddColumn<Guid>(
                name: "MovingScheduleId",
                table: "TotalReceipt",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceScheduleId",
                table: "FullReceipt",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServiceSchedule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MovingScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StayingScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovingSchedule_ServiceSchedule",
                        column: x => x.MovingScheduleId,
                        principalTable: "MovingSchedule",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StayingSchedule_ServiceSchedule",
                        column: x => x.StayingScheduleId,
                        principalTable: "StayingSchedule",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TotalReceipt_MovingScheduleId",
                table: "TotalReceipt",
                column: "MovingScheduleId",
                unique: true,
                filter: "[MovingScheduleId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TotalReceipt_StayingScheduleId",
                table: "TotalReceipt",
                column: "StayingScheduleId",
                unique: true,
                filter: "[StayingScheduleId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FullReceipt_ServiceScheduleId",
                table: "FullReceipt",
                column: "ServiceScheduleId",
                unique: true,
                filter: "[ServiceScheduleId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSchedule_MovingScheduleId",
                table: "ServiceSchedule",
                column: "MovingScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSchedule_StayingScheduleId",
                table: "ServiceSchedule",
                column: "StayingScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_FullReceipt_ServiceSchedule",
                table: "FullReceipt",
                column: "ServiceScheduleId",
                principalTable: "ServiceSchedule",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TotalReceipt_MovingSchedule_MovingScheduleId",
                table: "TotalReceipt",
                column: "MovingScheduleId",
                principalTable: "MovingSchedule",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TotalReceipt_StayingSchedule_StayingScheduleId",
                table: "TotalReceipt",
                column: "StayingScheduleId",
                principalTable: "StayingSchedule",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FullReceipt_ServiceSchedule",
                table: "FullReceipt");

            migrationBuilder.DropForeignKey(
                name: "FK_TotalReceipt_MovingSchedule_MovingScheduleId",
                table: "TotalReceipt");

            migrationBuilder.DropForeignKey(
                name: "FK_TotalReceipt_StayingSchedule_StayingScheduleId",
                table: "TotalReceipt");

            migrationBuilder.DropTable(
                name: "ServiceSchedule");

            migrationBuilder.DropIndex(
                name: "IX_TotalReceipt_MovingScheduleId",
                table: "TotalReceipt");

            migrationBuilder.DropIndex(
                name: "IX_TotalReceipt_StayingScheduleId",
                table: "TotalReceipt");

            migrationBuilder.DropIndex(
                name: "IX_FullReceipt_ServiceScheduleId",
                table: "FullReceipt");

            migrationBuilder.DropColumn(
                name: "MovingScheduleId",
                table: "TotalReceipt");

            migrationBuilder.DropColumn(
                name: "ServiceScheduleId",
                table: "FullReceipt");

            migrationBuilder.RenameColumn(
                name: "StayingScheduleId",
                table: "TotalReceipt",
                newName: "ScheduleId");

            migrationBuilder.AddColumn<int>(
                name: "ScheduleType",
                table: "TotalReceipt",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "StayingSchedule",
                type: "ntext",
                nullable: true,
                defaultValueSql: "''");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "StayingSchedule",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "StayingSchedule",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "StayingSchedule",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "MovingSchedule",
                type: "ntext",
                nullable: true,
                defaultValueSql: "''");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "MovingSchedule",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "MovingSchedule",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "MovingSchedule",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "EatSchedule",
                type: "ntext",
                nullable: true,
                defaultValueSql: "''");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "EatSchedule",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "EatSchedule",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "EatSchedule",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
