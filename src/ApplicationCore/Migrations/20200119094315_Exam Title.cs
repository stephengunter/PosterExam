using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationCore.Migrations
{
    public partial class ExamTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Exams",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Exams");
        }
    }
}
