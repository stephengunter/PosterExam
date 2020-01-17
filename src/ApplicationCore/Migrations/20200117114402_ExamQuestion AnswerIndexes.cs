using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationCore.Migrations
{
    public partial class ExamQuestionAnswerIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnswerIndex",
                table: "ExamQuestions");

            migrationBuilder.RenameColumn(
                name: "UserAnswerIndex",
                table: "ExamQuestions",
                newName: "UserAnswerIndexes");

            migrationBuilder.AddColumn<string>(
                name: "AnswerIndexes",
                table: "ExamQuestions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnswerIndexes",
                table: "ExamQuestions");

            migrationBuilder.RenameColumn(
                name: "UserAnswerIndexes",
                table: "ExamQuestions",
                newName: "UserAnswerIndex");

            migrationBuilder.AddColumn<int>(
                name: "AnswerIndex",
                table: "ExamQuestions",
                nullable: false,
                defaultValue: 0);
        }
    }
}
