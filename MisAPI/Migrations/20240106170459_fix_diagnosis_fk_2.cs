using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MisAPI.Migrations
{
    /// <inheritdoc />
    public partial class fix_diagnosis_fk_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diagnoses_Inspections_InspectionId",
                table: "Diagnoses");

            migrationBuilder.RenameColumn(
                name: "InspectionId",
                table: "Diagnoses",
                newName: "inspection_id");

            migrationBuilder.RenameIndex(
                name: "IX_Diagnoses_InspectionId",
                table: "Diagnoses",
                newName: "IX_Diagnoses_inspection_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Diagnoses_Inspections_inspection_id",
                table: "Diagnoses",
                column: "inspection_id",
                principalTable: "Inspections",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diagnoses_Inspections_inspection_id",
                table: "Diagnoses");

            migrationBuilder.RenameColumn(
                name: "inspection_id",
                table: "Diagnoses",
                newName: "InspectionId");

            migrationBuilder.RenameIndex(
                name: "IX_Diagnoses_inspection_id",
                table: "Diagnoses",
                newName: "IX_Diagnoses_InspectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Diagnoses_Inspections_InspectionId",
                table: "Diagnoses",
                column: "InspectionId",
                principalTable: "Inspections",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
