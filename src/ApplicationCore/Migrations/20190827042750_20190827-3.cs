using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationCore.Migrations
{
    public partial class _201908273 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Subjects_SubjectId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_SubjectId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Subjects");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "Subjects",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_SubjectId",
                table: "Subjects",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Subjects_SubjectId",
                table: "Subjects",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
