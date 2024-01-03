using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MisAPI.Migrations
{
    /// <inheritdoc />
    public partial class update_doctor_connections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Doctors_speciality_id",
                table: "Doctors",
                column: "speciality_id");
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropIndex(
                name: "IX_Doctors_speciality_id",
                table: "Doctors");
        }
    }
}
