using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationCore.Migrations
{
    public partial class PlanDefault : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pay_Bill_BillId",
                table: "Pay");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscribe_Bill_BillId",
                table: "Subscribe");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscribe_Plan_PlanId",
                table: "Subscribe");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscribe_AspNetUsers_UserId",
                table: "Subscribe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscribe",
                table: "Subscribe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Plan",
                table: "Plan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pay",
                table: "Pay");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bill",
                table: "Bill");

            migrationBuilder.RenameTable(
                name: "Subscribe",
                newName: "Subscribes");

            migrationBuilder.RenameTable(
                name: "Plan",
                newName: "Plans");

            migrationBuilder.RenameTable(
                name: "Pay",
                newName: "Pays");

            migrationBuilder.RenameTable(
                name: "Bill",
                newName: "Bills");

            migrationBuilder.RenameIndex(
                name: "IX_Subscribe_UserId",
                table: "Subscribes",
                newName: "IX_Subscribes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscribe_PlanId",
                table: "Subscribes",
                newName: "IX_Subscribes_PlanId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscribe_BillId",
                table: "Subscribes",
                newName: "IX_Subscribes_BillId");

            migrationBuilder.RenameIndex(
                name: "IX_Pay_BillId",
                table: "Pays",
                newName: "IX_Pays_BillId");

            migrationBuilder.AddColumn<bool>(
                name: "Default",
                table: "Plans",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Plans",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscribes",
                table: "Subscribes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Plans",
                table: "Plans",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pays",
                table: "Pays",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bills",
                table: "Bills",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Pays_Bills_BillId",
                table: "Pays",
                column: "BillId",
                principalTable: "Bills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscribes_Bills_BillId",
                table: "Subscribes",
                column: "BillId",
                principalTable: "Bills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscribes_Plans_PlanId",
                table: "Subscribes",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscribes_AspNetUsers_UserId",
                table: "Subscribes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pays_Bills_BillId",
                table: "Pays");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscribes_Bills_BillId",
                table: "Subscribes");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscribes_Plans_PlanId",
                table: "Subscribes");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscribes_AspNetUsers_UserId",
                table: "Subscribes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscribes",
                table: "Subscribes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Plans",
                table: "Plans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pays",
                table: "Pays");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bills",
                table: "Bills");

            migrationBuilder.DropColumn(
                name: "Default",
                table: "Plans");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Plans");

            migrationBuilder.RenameTable(
                name: "Subscribes",
                newName: "Subscribe");

            migrationBuilder.RenameTable(
                name: "Plans",
                newName: "Plan");

            migrationBuilder.RenameTable(
                name: "Pays",
                newName: "Pay");

            migrationBuilder.RenameTable(
                name: "Bills",
                newName: "Bill");

            migrationBuilder.RenameIndex(
                name: "IX_Subscribes_UserId",
                table: "Subscribe",
                newName: "IX_Subscribe_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscribes_PlanId",
                table: "Subscribe",
                newName: "IX_Subscribe_PlanId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscribes_BillId",
                table: "Subscribe",
                newName: "IX_Subscribe_BillId");

            migrationBuilder.RenameIndex(
                name: "IX_Pays_BillId",
                table: "Pay",
                newName: "IX_Pay_BillId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscribe",
                table: "Subscribe",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Plan",
                table: "Plan",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pay",
                table: "Pay",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bill",
                table: "Bill",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Pay_Bill_BillId",
                table: "Pay",
                column: "BillId",
                principalTable: "Bill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscribe_Bill_BillId",
                table: "Subscribe",
                column: "BillId",
                principalTable: "Bill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscribe_Plan_PlanId",
                table: "Subscribe",
                column: "PlanId",
                principalTable: "Plan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscribe_AspNetUsers_UserId",
                table: "Subscribe",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
