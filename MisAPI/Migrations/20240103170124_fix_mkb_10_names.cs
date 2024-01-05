using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MisAPI.Migrations
{
    /// <inheritdoc />
    public partial class fix_mkb_10_names : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id_uuid",
                table: "Mkb10",
                newName: "id_guid");

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "Diagnoses",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id_guid",
                table: "Mkb10",
                newName: "id_uuid");

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "Diagnoses",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
