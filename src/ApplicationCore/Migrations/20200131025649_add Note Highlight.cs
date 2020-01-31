using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationCore.Migrations
{
    public partial class addNoteHighlight : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Highlight",
                table: "Notes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Notes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Highlight",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Notes");
        }
    }
}
