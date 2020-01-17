using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationCore.Migrations
{
    public partial class ExamQuestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExamType",
                table: "Exams",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RecruitExamType",
                table: "Exams",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "Exams",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Exams",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ExamParts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExamId = table.Column<int>(nullable: false),
                    Points = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamParts_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExamPartId = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    QuestionId = table.Column<int>(nullable: false),
                    AnswerIndex = table.Column<int>(nullable: false),
                    OptionIndexes = table.Column<string>(nullable: true),
                    UserAnswerIndex = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamQuestions_ExamParts_ExamPartId",
                        column: x => x.ExamPartId,
                        principalTable: "ExamParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamQuestions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamParts_ExamId",
                table: "ExamParts",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestions_ExamPartId",
                table: "ExamQuestions",
                column: "ExamPartId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamQuestions_QuestionId",
                table: "ExamQuestions",
                column: "QuestionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamQuestions");

            migrationBuilder.DropTable(
                name: "ExamParts");

            migrationBuilder.DropColumn(
                name: "ExamType",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "RecruitExamType",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Exams");
        }
    }
}
