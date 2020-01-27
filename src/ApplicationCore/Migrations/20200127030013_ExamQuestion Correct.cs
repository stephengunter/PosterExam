using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationCore.Migrations
{
    public partial class ExamQuestionCorrect : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestOK",
                table: "Exams");

            migrationBuilder.AddColumn<bool>(
                name: "Correct",
                table: "ExamQuestions",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Correct",
                table: "ExamQuestions");

            migrationBuilder.AddColumn<bool>(
                name: "TestOK",
                table: "Exams",
                nullable: false,
                defaultValue: false);
        }
    }
}
