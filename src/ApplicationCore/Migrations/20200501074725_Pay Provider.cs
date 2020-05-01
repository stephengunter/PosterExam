using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationCore.Migrations
{
    public partial class PayProvider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayWayId",
                table: "Pays");

            migrationBuilder.AddColumn<string>(
                name: "PayWay",
                table: "Pays",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Provider",
                table: "Pays",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayWay",
                table: "Pays");

            migrationBuilder.DropColumn(
                name: "Provider",
                table: "Pays");

            migrationBuilder.AddColumn<int>(
                name: "PayWayId",
                table: "Pays",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
