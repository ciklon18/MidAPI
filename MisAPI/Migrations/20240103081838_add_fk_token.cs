using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MisAPI.Migrations
{
    /// <inheritdoc />
    public partial class add_fk_token : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionComment_DoctorModel_AuthorId",
                table: "InspectionComment");

            migrationBuilder.DropTable(
                name: "DoctorModel");

            migrationBuilder.CreateIndex(
                name: "IX_Refresh tokens_doctorId",
                table: "Refresh tokens",
                column: "doctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionComment_Doctors_AuthorId",
                table: "InspectionComment",
                column: "AuthorId",
                principalTable: "Doctors",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Refresh tokens_Doctors_doctorId",
                table: "Refresh tokens",
                column: "doctorId",
                principalTable: "Doctors",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionComment_Doctors_AuthorId",
                table: "InspectionComment");

            migrationBuilder.DropForeignKey(
                name: "FK_Refresh tokens_Doctors_doctorId",
                table: "Refresh tokens");

            migrationBuilder.DropIndex(
                name: "IX_Refresh tokens_doctorId",
                table: "Refresh tokens");

            migrationBuilder.CreateTable(
                name: "DoctorModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Birthday = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorModel", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionComment_DoctorModel_AuthorId",
                table: "InspectionComment",
                column: "AuthorId",
                principalTable: "DoctorModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
