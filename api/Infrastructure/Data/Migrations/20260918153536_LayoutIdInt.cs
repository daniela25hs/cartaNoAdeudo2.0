using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CartaNoAdeudoApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class LayoutIdInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // uuid -> integer no tiene cast implícito en Postgres; se recrea la columna en
            // vez de ALTER TYPE (mismo enfoque que 20260917214246_EstadoTareaIdInt). layout
            // no tiene FKs dependientes.
            migrationBuilder.DropPrimaryKey(
                name: "PK_layout",
                schema: "cat",
                table: "layout");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "cat",
                table: "layout");

            migrationBuilder.AddColumn<int>(
                    name: "id",
                    schema: "cat",
                    table: "layout",
                    type: "integer",
                    nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_layout",
                schema: "cat",
                table: "layout",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_layout",
                schema: "cat",
                table: "layout");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "cat",
                table: "layout");

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                schema: "cat",
                table: "layout",
                type: "uuid",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_layout",
                schema: "cat",
                table: "layout",
                column: "id");
        }
    }
}
