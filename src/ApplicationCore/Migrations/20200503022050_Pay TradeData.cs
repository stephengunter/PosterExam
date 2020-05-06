using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationCore.Migrations
{
    public partial class PayTradeData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PayedDate",
                table: "Pays",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TradeData",
                table: "Pays",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayedDate",
                table: "Pays");

            migrationBuilder.DropColumn(
                name: "TradeData",
                table: "Pays");
        }
    }
}
