using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LekkoApp.Migrations
{
    /// <inheritdoc />
    public partial class PomodoroSessionNewTestingVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pomodoros_Tasks_TaskId",
                table: "Pomodoros");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Pomodoros");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Pomodoros");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Pomodoros",
                newName: "StartedAt");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Pomodoros",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "TaskId",
                table: "Pomodoros",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndedAt",
                table: "Pomodoros",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InterruptReason",
                table: "Pomodoros",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlannedDurationMinutes",
                table: "Pomodoros",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Pomodoros",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Pomodoros_Tasks_TaskId",
                table: "Pomodoros",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pomodoros_Tasks_TaskId",
                table: "Pomodoros");

            migrationBuilder.DropColumn(
                name: "EndedAt",
                table: "Pomodoros");

            migrationBuilder.DropColumn(
                name: "InterruptReason",
                table: "Pomodoros");

            migrationBuilder.DropColumn(
                name: "PlannedDurationMinutes",
                table: "Pomodoros");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Pomodoros");

            migrationBuilder.RenameColumn(
                name: "StartedAt",
                table: "Pomodoros",
                newName: "StartTime");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Pomodoros",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TaskId",
                table: "Pomodoros",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Pomodoros",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Pomodoros",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Pomodoros_Tasks_TaskId",
                table: "Pomodoros",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id");
        }
    }
}
