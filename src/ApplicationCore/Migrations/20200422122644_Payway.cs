using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationCore.Migrations
{
    public partial class Payway : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayWay",
                table: "Pays");

            migrationBuilder.DropColumn(
                name: "PayWay",
                table: "Bills");

            migrationBuilder.AddColumn<int>(
                name: "PayWayId",
                table: "Pays",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PayWayId",
                table: "Bills",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PayWay",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false),
                    Removed = table.Column<bool>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayWay", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pays_PayWayId",
                table: "Pays",
                column: "PayWayId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pays_PayWay_PayWayId",
                table: "Pays",
                column: "PayWayId",
                principalTable: "PayWay",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pays_PayWay_PayWayId",
                table: "Pays");

            migrationBuilder.DropTable(
                name: "PayWay");

            migrationBuilder.DropIndex(
                name: "IX_Pays_PayWayId",
                table: "Pays");

            migrationBuilder.DropColumn(
                name: "PayWayId",
                table: "Pays");

            migrationBuilder.DropColumn(
                name: "PayWayId",
                table: "Bills");

            migrationBuilder.AddColumn<int>(
                name: "PayWay",
                table: "Pays",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PayWay",
                table: "Bills",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
