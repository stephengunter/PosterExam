using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationCore.Migrations
{
    public partial class RecruitSubjectId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecruitSubjects");

            migrationBuilder.DropColumn(
                name: "SubjectIds",
                table: "Recruits");

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "Recruits",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Recruits");

            migrationBuilder.AddColumn<string>(
                name: "SubjectIds",
                table: "Recruits",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RecruitSubjects",
                columns: table => new
                {
                    RecruitId = table.Column<int>(nullable: false),
                    SubjectId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecruitSubjects", x => new { x.RecruitId, x.SubjectId });
                });
        }
    }
}
