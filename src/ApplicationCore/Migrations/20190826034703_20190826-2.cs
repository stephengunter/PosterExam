using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationCore.Migrations
{
    public partial class _201908262 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Terms",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Terms",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Text",
                table: "Terms");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Terms");
        }
    }
}
