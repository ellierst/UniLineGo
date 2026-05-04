using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniLineGo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReminderToTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReminderMinutes",
                table: "Tasks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReminderSent",
                table: "Tasks",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReminderMinutes",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "ReminderSent",
                table: "Tasks");
        }
    }
}