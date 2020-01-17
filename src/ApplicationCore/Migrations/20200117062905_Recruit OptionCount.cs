using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationCore.Migrations
{
    public partial class RecruitOptionCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MultiAnswers",
                table: "Recruits",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OptionCount",
                table: "Recruits",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MultiAnswers",
                table: "Recruits");

            migrationBuilder.DropColumn(
                name: "OptionCount",
                table: "Recruits");
        }
    }
}
