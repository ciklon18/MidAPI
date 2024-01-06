using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MisAPI.Migrations
{
    /// <inheritdoc />
    public partial class renamed_from_mkb10_to_icd_10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mkb10");

            migrationBuilder.DropTable(
                name: "Mkb10Roots");

            migrationBuilder.AddColumn<Guid>(
                name: "icd_diagnosis_id",
                table: "Diagnoses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Icd10",
                columns: table => new
                {
                    id_guid = table.Column<Guid>(type: "uuid", nullable: false),
                    id_int = table.Column<int>(type: "integer", nullable: false),
                    root_id_int = table.Column<int>(type: "integer", nullable: true),
                    root_id_guid = table.Column<Guid>(type: "uuid", nullable: true),
                    icd_code = table.Column<string>(type: "text", nullable: false),
                    icd_name = table.Column<string>(type: "text", nullable: false),
                    rec_code = table.Column<string>(type: "text", nullable: false),
                    addl_code = table.Column<string>(type: "text", nullable: true),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    parent_id = table.Column<int>(type: "integer", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Icd10", x => x.id_guid);
                });

            migrationBuilder.CreateTable(
                name: "Icd10Roots",
                columns: table => new
                {
                    key_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Icd10Roots", x => x.key_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Icd10");

            migrationBuilder.DropTable(
                name: "Icd10Roots");

            migrationBuilder.DropColumn(
                name: "icd_diagnosis_id",
                table: "Diagnoses");

            migrationBuilder.CreateTable(
                name: "Mkb10",
                columns: table => new
                {
                    id_guid = table.Column<Guid>(type: "uuid", nullable: false),
                    addl_code = table.Column<string>(type: "text", nullable: true),
                    create_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    id_int = table.Column<int>(type: "integer", nullable: false),
                    parent_id = table.Column<int>(type: "integer", nullable: false),
                    mkb_code = table.Column<string>(type: "text", nullable: false),
                    mkb_name = table.Column<string>(type: "text", nullable: false),
                    rec_code = table.Column<string>(type: "text", nullable: false),
                    root_id_guid = table.Column<Guid>(type: "uuid", nullable: true),
                    root_id_int = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mkb10", x => x.id_guid);
                });

            migrationBuilder.CreateTable(
                name: "Mkb10Roots",
                columns: table => new
                {
                    key_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: false),
                    create_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mkb10Roots", x => x.key_id);
                });
        }
    }
}
