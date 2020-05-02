using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationCore.Migrations
{
    public partial class PayBankAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Account",
                table: "Pays");

            migrationBuilder.AddColumn<string>(
                name: "BankAccount",
                table: "Pays",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankCode",
                table: "Pays",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TradeNo",
                table: "Pays",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankAccount",
                table: "Pays");

            migrationBuilder.DropColumn(
                name: "BankCode",
                table: "Pays");

            migrationBuilder.DropColumn(
                name: "TradeNo",
                table: "Pays");

            migrationBuilder.AddColumn<string>(
                name: "Account",
                table: "Pays",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
