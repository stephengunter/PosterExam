using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationCore.Migrations
{
    public partial class ManualFree : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Free",
                table: "Manuals",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Free",
                table: "Manuals");
        }
    }
}
