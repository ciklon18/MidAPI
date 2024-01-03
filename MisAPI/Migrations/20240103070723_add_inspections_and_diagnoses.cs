using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MisAPI.Migrations
{
    /// <inheritdoc />
    public partial class add_inspections_and_diagnoses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DoctorModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Birthday = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    birthday = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    gender = table.Column<int>(type: "integer", nullable: false),
                    doctor_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "InspectionComment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifyTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspectionComment_DoctorModel_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "DoctorModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inspections",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    anamnesis = table.Column<string>(type: "text", nullable: true),
                    complaints = table.Column<string>(type: "text", nullable: true),
                    treatment = table.Column<string>(type: "text", nullable: true),
                    conclusion = table.Column<int>(type: "integer", nullable: false),
                    next_visit_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    death_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    base_inspection_id = table.Column<Guid>(type: "uuid", nullable: true),
                    previous_inspection_id = table.Column<Guid>(type: "uuid", nullable: true),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    doctor_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspections", x => x.id);
                    table.ForeignKey(
                        name: "FK_Inspections_Doctors_doctor_id",
                        column: x => x.doctor_id,
                        principalTable: "Doctors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inspections_Patients_patient_id",
                        column: x => x.patient_id,
                        principalTable: "Patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Diagnoses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    code = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    InspectionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnoses", x => x.id);
                    table.ForeignKey(
                        name: "FK_Diagnoses_Inspections_InspectionId",
                        column: x => x.InspectionId,
                        principalTable: "Inspections",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "InspectionConsultations",
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
                });

            migrationBuilder.CreateIndex(
                name: "IX_Diagnoses_InspectionId",
                table: "Diagnoses",
                column: "InspectionId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionComment_AuthorId",
                table: "InspectionComment",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionConsultations_inspection_id",
                table: "InspectionConsultations",
                column: "inspection_id");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionConsultations_root_comment_id",
                table: "InspectionConsultations",
                column: "root_comment_id");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_doctor_id",
                table: "Inspections",
                column: "doctor_id");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_patient_id",
                table: "Inspections",
                column: "patient_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Diagnoses");

            migrationBuilder.DropTable(
                name: "InspectionConsultations");

            migrationBuilder.DropTable(
                name: "InspectionComment");

            migrationBuilder.DropTable(
                name: "Inspections");

            migrationBuilder.DropTable(
                name: "DoctorModel");

            migrationBuilder.DropTable(
                name: "Patients");
        }
    }
}
