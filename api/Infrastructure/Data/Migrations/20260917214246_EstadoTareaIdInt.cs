using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CartaNoAdeudoApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class EstadoTareaIdInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // uuid -> integer no tiene cast implícito en Postgres; se recrean las columnas
            // en vez de ALTER TYPE. estado_tarea conserva sus filas (el id nuevo se
            // autogenera en el mismo orden), tareas está vacía en este punto del desarrollo.
            migrationBuilder.DropForeignKey(
                name: "FK_tareas_estado_tarea_id_estado",
                schema: "carta",
                table: "tareas");

            migrationBuilder.DropColumn(
                name: "id_estado",
                schema: "carta",
                table: "tareas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_estado_tarea",
                schema: "cat",
                table: "estado_tarea");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "cat",
                table: "estado_tarea");

            migrationBuilder.AddColumn<int>(
                    name: "id",
                    schema: "cat",
                    table: "estado_tarea",
                    type: "integer",
                    nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_estado_tarea",
                schema: "cat",
                table: "estado_tarea",
                column: "id");

            migrationBuilder.AddColumn<int>(
                name: "id_estado",
                schema: "carta",
                table: "tareas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_tareas_id_estado",
                schema: "carta",
                table: "tareas",
                column: "id_estado");

            migrationBuilder.AddForeignKey(
                name: "FK_tareas_estado_tarea_id_estado",
                schema: "carta",
                table: "tareas",
                column: "id_estado",
                principalSchema: "cat",
                principalTable: "estado_tarea",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tareas_estado_tarea_id_estado",
                schema: "carta",
                table: "tareas");

            migrationBuilder.DropColumn(
                name: "id_estado",
                schema: "carta",
                table: "tareas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_estado_tarea",
                schema: "cat",
                table: "estado_tarea");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "cat",
                table: "estado_tarea");

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                schema: "cat",
                table: "estado_tarea",
                type: "uuid",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_estado_tarea",
                schema: "cat",
                table: "estado_tarea",
                column: "id");

            migrationBuilder.AddColumn<Guid>(
                name: "id_estado",
                schema: "carta",
                table: "tareas",
                type: "uuid",
                nullable: false);

            migrationBuilder.AddForeignKey(
                name: "FK_tareas_estado_tarea_id_estado",
                schema: "carta",
                table: "tareas",
                column: "id_estado",
                principalSchema: "cat",
                principalTable: "estado_tarea",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
