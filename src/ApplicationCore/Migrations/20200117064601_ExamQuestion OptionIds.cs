using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationCore.Migrations
{
    public partial class ExamQuestionOptionIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OptionIndexes",
                table: "ExamQuestions",
                newName: "OptionIds");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OptionIds",
                table: "ExamQuestions",
                newName: "OptionIndexes");
        }
    }
}
