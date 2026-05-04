using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniLineGo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReminderToTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "reminder_minutes",
                table: "tasks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "reminder_sent",
                table: "tasks",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reminder_minutes",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "reminder_sent",
                table: "tasks");
        }
    }
}
