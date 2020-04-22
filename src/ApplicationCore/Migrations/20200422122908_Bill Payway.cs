using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationCore.Migrations
{
    public partial class BillPayway : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pays_PayWay_PayWayId",
                table: "Pays");

            migrationBuilder.DropIndex(
                name: "IX_Pays_PayWayId",
                table: "Pays");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_PayWayId",
                table: "Bills",
                column: "PayWayId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bills_PayWay_PayWayId",
                table: "Bills",
                column: "PayWayId",
                principalTable: "PayWay",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bills_PayWay_PayWayId",
                table: "Bills");

            migrationBuilder.DropIndex(
                name: "IX_Bills_PayWayId",
                table: "Bills");

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
    }
}
