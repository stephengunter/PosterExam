using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationCore.Migrations
{
    public partial class ExamPartOptionCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OptionCount",
                table: "Exams");

            migrationBuilder.AddColumn<int>(
                name: "OptionCount",
                table: "ExamParts",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OptionCount",
                table: "ExamParts");

            migrationBuilder.AddColumn<int>(
                name: "OptionCount",
                table: "Exams",
                nullable: false,
                defaultValue: 0);
        }
    }
}
