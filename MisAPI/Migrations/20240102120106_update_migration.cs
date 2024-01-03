using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MisAPI.Migrations
{
    /// <inheritdoc />
    public partial class update_migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "full_name",
                table: "Doctors",
                newName: "name");
            migrationBuilder.RenameColumn(
                name: "birth_date",
                table: "Doctors",
                newName: "birthday");

            migrationBuilder.CreateTable(
                name: "Mkb10",
                columns: table => new
                {
                    id_int = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_uuid = table.Column<Guid>(type: "uuid", nullable: true),
                    root_id_int = table.Column<int>(type: "integer", nullable: true),
                    root_id_guid = table.Column<Guid>(type: "uuid", nullable: true),
                    mkb_code = table.Column<string>(type: "text", nullable: false),
                    mkb_name = table.Column<string>(type: "text", nullable: false),
                    rec_code = table.Column<string>(type: "text", nullable: false),
                    addl_code = table.Column<string>(type: "text", nullable: true),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    parent_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mkb10", x => x.id_int);
                });
            
            migrationBuilder.CreateTable(
                name: "Specialities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialities", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Doctors");

            migrationBuilder.DropTable(
                name: "Mkb10");

            migrationBuilder.DropTable(
                name: "Refresh tokens");

            migrationBuilder.DropTable(
                name: "Specialities");
        }
    }
}
