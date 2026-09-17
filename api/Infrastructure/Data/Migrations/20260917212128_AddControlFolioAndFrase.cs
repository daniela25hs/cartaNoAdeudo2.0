using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CartaNoAdeudoApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddControlFolioAndFrase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "control_folios",
                schema: "sist",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    anio = table.Column<int>(type: "integer", nullable: false),
                    ultimo_folio = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_control_folios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "frases",
                schema: "cat",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    anio = table.Column<int>(type: "integer", nullable: false),
                    frase = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    fecha_inicio_autorizada = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_fin_autorizada = table.Column<DateOnly>(type: "date", nullable: false),
                    id_creo = table.Column<Guid>(type: "uuid", nullable: true),
                    fecha_creo = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    id_edito = table.Column<Guid>(type: "uuid", nullable: true),
                    fecha_edito = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_frases", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_control_folios_anio",
                schema: "sist",
                table: "control_folios",
                column: "anio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_frases_anio",
                schema: "cat",
                table: "frases",
                column: "anio");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "control_folios",
                schema: "sist");

            migrationBuilder.DropTable(
                name: "frases",
                schema: "cat");
        }
    }
}
