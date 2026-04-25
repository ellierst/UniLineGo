using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniLineGo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Додаємо колонку як nullable спочатку
            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "tasks",
                type: "integer",
                nullable: true);
        
            // 2. Видаляємо старі завдання без власника (вони все одно нікому не належать)
            migrationBuilder.Sql("DELETE FROM tasks WHERE user_id IS NULL;");
        
            // 3. Робимо колонку NOT NULL
            migrationBuilder.AlterColumn<int>(
                name: "user_id",
                table: "tasks",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        
            // 4. Додаємо FK
            migrationBuilder.AddForeignKey(
                name: "FK_tasks_users_user_id",
                table: "tasks",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        
            // 5. Індекс
            migrationBuilder.CreateIndex(
                name: "IX_tasks_user_id",
                table: "tasks",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tasks_users_user_id",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "IX_tasks_user_id",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "tasks");
        }
    }
}
