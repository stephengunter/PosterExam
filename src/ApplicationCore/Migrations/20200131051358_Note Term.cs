using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationCore.Migrations
{
    public partial class NoteTerm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Notes_TermId",
                table: "Notes",
                column: "TermId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Terms_TermId",
                table: "Notes",
                column: "TermId",
                principalTable: "Terms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Terms_TermId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_TermId",
                table: "Notes");
        }
    }
}
