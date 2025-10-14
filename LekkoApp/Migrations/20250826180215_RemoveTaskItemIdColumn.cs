using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LekkoApp.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTaskItemIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaskItemId",
                table: "Pomodoros");

            migrationBuilder.RenameColumn(
                name: "Completed",
                table: "Pomodoros",
                newName: "IsCompleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsCompleted",
                table: "Pomodoros",
                newName: "Completed");

            migrationBuilder.AddColumn<Guid>(
                name: "TaskItemId",
                table: "Pomodoros",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
