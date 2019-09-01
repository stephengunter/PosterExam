using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationCore.Migrations
{
    public partial class _201908311 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TermIds",
                table: "Questions");

            migrationBuilder.AddColumn<int>(
                name: "TermId",
                table: "Questions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TermId",
                table: "Questions");

            migrationBuilder.AddColumn<string>(
                name: "TermIds",
                table: "Questions",
                nullable: true);
        }
    }
}
