using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationCore.Migrations
{
    public partial class SubscribeBill : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscribes_Plans_PlanId",
                table: "Subscribes");

            migrationBuilder.AlterColumn<int>(
                name: "PlanId",
                table: "Subscribes",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "HasDiscount",
                table: "Bills",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PlanId",
                table: "Bills",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bills_PlanId",
                table: "Bills",
                column: "PlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bills_Plans_PlanId",
                table: "Bills",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscribes_Plans_PlanId",
                table: "Subscribes",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bills_Plans_PlanId",
                table: "Bills");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscribes_Plans_PlanId",
                table: "Subscribes");

            migrationBuilder.DropIndex(
                name: "IX_Bills_PlanId",
                table: "Bills");

            migrationBuilder.DropColumn(
                name: "HasDiscount",
                table: "Bills");

            migrationBuilder.DropColumn(
                name: "PlanId",
                table: "Bills");

            migrationBuilder.AlterColumn<int>(
                name: "PlanId",
                table: "Subscribes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscribes_Plans_PlanId",
                table: "Subscribes",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
