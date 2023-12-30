using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MisAPI.Migrations
{
    /// <inheritdoc />
    public partial class FirstMighration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_refresh_tokens",
                table: "refresh_tokens");

            migrationBuilder.RenameTable(
                name: "refresh_tokens",
                newName: "Refresh tokens");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Refresh tokens",
                table: "Refresh tokens",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Refresh tokens",
                table: "Refresh tokens");

            migrationBuilder.RenameTable(
                name: "Refresh tokens",
                newName: "refresh_tokens");

            migrationBuilder.AddPrimaryKey(
                name: "PK_refresh_tokens",
                table: "refresh_tokens",
                column: "id");
        }
    }
}
