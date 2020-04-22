using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationCore.Migrations
{
    public partial class PlanDiscount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Default",
                table: "Plans");

            migrationBuilder.AddColumn<int>(
                name: "Discount",
                table: "Plans",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Plans");

            migrationBuilder.AddColumn<bool>(
                name: "Default",
                table: "Plans",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
