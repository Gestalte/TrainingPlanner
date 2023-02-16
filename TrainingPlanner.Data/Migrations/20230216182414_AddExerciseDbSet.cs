using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingPlanner.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_Schedules_ScheduleId",
                table: "Exercise");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Exercise",
                table: "Exercise");

            migrationBuilder.RenameTable(
                name: "Exercise",
                newName: "Exercises");

            migrationBuilder.RenameIndex(
                name: "IX_Exercise_ScheduleId",
                table: "Exercises",
                newName: "IX_Exercises_ScheduleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Exercises",
                table: "Exercises",
                column: "ExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercises_Schedules_ScheduleId",
                table: "Exercises",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "ScheduleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercises_Schedules_ScheduleId",
                table: "Exercises");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Exercises",
                table: "Exercises");

            migrationBuilder.RenameTable(
                name: "Exercises",
                newName: "Exercise");

            migrationBuilder.RenameIndex(
                name: "IX_Exercises_ScheduleId",
                table: "Exercise",
                newName: "IX_Exercise_ScheduleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Exercise",
                table: "Exercise",
                column: "ExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercise_Schedules_ScheduleId",
                table: "Exercise",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "ScheduleId");
        }
    }
}
