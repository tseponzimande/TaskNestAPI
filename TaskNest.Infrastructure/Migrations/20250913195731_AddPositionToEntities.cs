﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskNest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPositionToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "TaskItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "TaskItems");
        }
    }
}
