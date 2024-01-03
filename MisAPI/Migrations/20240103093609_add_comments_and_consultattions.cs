using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MisAPI.Migrations
{
    /// <inheritdoc />
    public partial class add_comments_and_consultattions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InspectionConsultations");

            migrationBuilder.DropTable(
                name: "InspectionComment");

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    content = table.Column<string>(type: "text", nullable: true),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    modify_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    consultation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    CommentId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.id);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Comments_Doctors_author_id",
                        column: x => x.author_id,
                        principalTable: "Doctors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Consultations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    inspection_id = table.Column<Guid>(type: "uuid", nullable: false),
                    speciality_id = table.Column<Guid>(type: "uuid", nullable: false),
                    root_comment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    comments_number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultations", x => x.id);
                    table.ForeignKey(
                        name: "FK_Consultations_Comments_root_comment_id",
                        column: x => x.root_comment_id,
                        principalTable: "Comments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Consultations_Inspections_inspection_id",
                        column: x => x.inspection_id,
                        principalTable: "Inspections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Consultations_Specialities_speciality_id",
                        column: x => x.speciality_id,
                        principalTable: "Specialities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_author_id",
                table: "Comments",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentId",
                table: "Comments",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Consultations_inspection_id",
                table: "Consultations",
                column: "inspection_id");

            migrationBuilder.CreateIndex(
                name: "IX_Consultations_root_comment_id",
                table: "Consultations",
                column: "root_comment_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Consultations_speciality_id",
                table: "Consultations",
                column: "speciality_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Consultations");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.CreateTable(
                name: "InspectionComment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsultationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InspectionCommentId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifyTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionComment_Doctors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Doctors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InspectionComment_InspectionComment_InspectionCommentId",
                        column: x => x.InspectionCommentId,
                        principalTable: "InspectionComment",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InspectionConsultations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    inspection_id = table.Column<Guid>(type: "uuid", nullable: false),
                    root_comment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    speciality_id = table.Column<Guid>(type: "uuid", nullable: false),
                    comments_number = table.Column<int>(type: "integer", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionConsultations", x => x.id);
                    table.ForeignKey(
                        name: "FK_InspectionConsultations_InspectionComment_root_comment_id",
                        column: x => x.root_comment_id,
                        principalTable: "InspectionComment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InspectionConsultations_Inspections_inspection_id",
                        column: x => x.inspection_id,
                        principalTable: "Inspections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InspectionConsultations_Specialities_speciality_id",
                        column: x => x.speciality_id,
                        principalTable: "Specialities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionComment_AuthorId",
                table: "InspectionComment",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionComment_InspectionCommentId",
                table: "InspectionComment",
                column: "InspectionCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionConsultations_inspection_id",
                table: "InspectionConsultations",
                column: "inspection_id");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionConsultations_root_comment_id",
                table: "InspectionConsultations",
                column: "root_comment_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InspectionConsultations_speciality_id",
                table: "InspectionConsultations",
                column: "speciality_id");
        }
    }
}
