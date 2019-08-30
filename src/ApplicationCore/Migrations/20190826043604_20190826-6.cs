using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationCore.Migrations
{
    public partial class _201908266 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Terms",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "Terms",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Terms_SubjectId",
                table: "Terms",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Terms_Subjects_SubjectId",
                table: "Terms",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Terms_Subjects_SubjectId",
                table: "Terms");

            migrationBuilder.DropIndex(
                name: "IX_Terms_SubjectId",
                table: "Terms");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Terms");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Terms");
        }
    }
}
