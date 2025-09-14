using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskNest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBoardColumnsSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ColumnId",
                table: "TaskItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BoardColumns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    BoardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardColumns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoardColumns_Boards_BoardId",
                        column: x => x.BoardId,
                        principalTable: "Boards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_ColumnId",
                table: "TaskItems",
                column: "ColumnId");

            migrationBuilder.CreateIndex(
                name: "IX_BoardColumns_BoardId",
                table: "BoardColumns",
                column: "BoardId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItems_BoardColumns_ColumnId",
                table: "TaskItems",
                column: "ColumnId",
                principalTable: "BoardColumns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItems_BoardColumns_ColumnId",
                table: "TaskItems");

            migrationBuilder.DropTable(
                name: "BoardColumns");

            migrationBuilder.DropIndex(
                name: "IX_TaskItems_ColumnId",
                table: "TaskItems");

            migrationBuilder.DropColumn(
                name: "ColumnId",
                table: "TaskItems");
        }
    }
}
