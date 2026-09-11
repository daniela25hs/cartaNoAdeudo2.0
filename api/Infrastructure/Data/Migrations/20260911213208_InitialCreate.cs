using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CartaNoAdeudoApi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "carta");

            migrationBuilder.EnsureSchema(
                name: "bit");

            migrationBuilder.EnsureSchema(
                name: "sist");

            migrationBuilder.EnsureSchema(
                name: "cat");

            migrationBuilder.CreateTable(
                name: "config",
                schema: "bit",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    accion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    fecha = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_config", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "configuracion",
                schema: "sist",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    clave = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    valor = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    fecha_modificacion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configuracion", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "estado",
                schema: "cat",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    descripcion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estado", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "estado_tarea",
                schema: "cat",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    descripcion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_estado_tarea", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "firmante",
                schema: "cat",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    puesto = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    genero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    certificado = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    pfx = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    contrasena = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    estatus_certificado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    fecha_inicio_certificado = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_fin_certificado = table.Column<DateOnly>(type: "date", nullable: false),
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
                    table.PrimaryKey("PK_firmante", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "folios_consecutivos",
                schema: "carta",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_carta = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ultimo_folio = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    id_creo = table.Column<Guid>(type: "uuid", nullable: true),
                    fecha_creo = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    id_edito = table.Column<Guid>(type: "uuid", nullable: true),
                    fecha_edito = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_folios_consecutivos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "layout",
                schema: "cat",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    archivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    fecha_hora = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
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
                    table.PrimaryKey("PK_layout", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tipos_cartas",
                schema: "carta",
                columns: table => new
                {
                    id_tipo_carta = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descripcion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    clave = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipos_cartas", x => x.id_tipo_carta);
                });

            migrationBuilder.CreateTable(
                name: "datos",
                schema: "carta",
                columns: table => new
                {
                    id_dato = table.Column<Guid>(type: "uuid", nullable: false),
                    id_tipo_carta = table.Column<int>(type: "integer", nullable: false),
                    nombre = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    rfc = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    ro = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    inicio_vigencia = table.Column<DateOnly>(type: "date", nullable: false),
                    vencimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    vencida = table.Column<bool>(type: "boolean", nullable: false),
                    alcoholes = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    folio = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    fecha_firmado = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    fecha_hora = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    estatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_datos", x => x.id_dato);
                    table.ForeignKey(
                        name: "FK_datos_tipos_cartas_id_tipo_carta",
                        column: x => x.id_tipo_carta,
                        principalSchema: "carta",
                        principalTable: "tipos_cartas",
                        principalColumn: "id_tipo_carta",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "archivos",
                schema: "carta",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    id_dato = table.Column<Guid>(type: "uuid", nullable: false),
                    carta = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_archivos", x => x.id);
                    table.ForeignKey(
                        name: "FK_archivos_datos_id_dato",
                        column: x => x.id_dato,
                        principalSchema: "carta",
                        principalTable: "datos",
                        principalColumn: "id_dato",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "carta",
                schema: "bit",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    id_dato = table.Column<Guid>(type: "uuid", nullable: false),
                    accion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    fecha_hora = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carta", x => x.id);
                    table.ForeignKey(
                        name: "FK_carta_datos_id_dato",
                        column: x => x.id_dato,
                        principalSchema: "carta",
                        principalTable: "datos",
                        principalColumn: "id_dato",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "firmas",
                schema: "carta",
                columns: table => new
                {
                    id_firma = table.Column<Guid>(type: "uuid", nullable: false),
                    id_dato = table.Column<Guid>(type: "uuid", nullable: false),
                    descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    fecha = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    identificador = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    certificado = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    hex_serie = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    finger_print = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    firma = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_firmas", x => x.id_firma);
                    table.ForeignKey(
                        name: "FK_firmas_datos_id_dato",
                        column: x => x.id_dato,
                        principalSchema: "carta",
                        principalTable: "datos",
                        principalColumn: "id_dato",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tareas",
                schema: "carta",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    id_dato = table.Column<Guid>(type: "uuid", nullable: false),
                    id_estado = table.Column<Guid>(type: "uuid", nullable: false),
                    fecha_inicio = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    fecha_fin = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    nota = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    mensaje_error = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tareas", x => x.id);
                    table.ForeignKey(
                        name: "FK_tareas_datos_id_dato",
                        column: x => x.id_dato,
                        principalSchema: "carta",
                        principalTable: "datos",
                        principalColumn: "id_dato",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tareas_estado_tarea_id_estado",
                        column: x => x.id_estado,
                        principalSchema: "cat",
                        principalTable: "estado_tarea",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_archivos_id_dato",
                schema: "carta",
                table: "archivos",
                column: "id_dato",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_carta_id_dato",
                schema: "bit",
                table: "carta",
                column: "id_dato");

            migrationBuilder.CreateIndex(
                name: "IX_configuracion_clave",
                schema: "sist",
                table: "configuracion",
                column: "clave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_datos_id_tipo_carta",
                schema: "carta",
                table: "datos",
                column: "id_tipo_carta");

            migrationBuilder.CreateIndex(
                name: "IX_datos_ro_id_tipo_carta",
                schema: "carta",
                table: "datos",
                columns: new[] { "ro", "id_tipo_carta" });

            migrationBuilder.CreateIndex(
                name: "IX_estado_descripcion",
                schema: "cat",
                table: "estado",
                column: "descripcion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_estado_tarea_descripcion",
                schema: "cat",
                table: "estado_tarea",
                column: "descripcion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_firmante_activo_fecha_inicio_autorizada_fecha_fin_autorizada",
                schema: "cat",
                table: "firmante",
                columns: new[] { "activo", "fecha_inicio_autorizada", "fecha_fin_autorizada" });

            migrationBuilder.CreateIndex(
                name: "IX_firmas_id_dato",
                schema: "carta",
                table: "firmas",
                column: "id_dato");

            migrationBuilder.CreateIndex(
                name: "IX_folios_consecutivos_tipo_carta",
                schema: "carta",
                table: "folios_consecutivos",
                column: "tipo_carta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_layout_activo_fecha_inicio_autorizada_fecha_fin_autorizada",
                schema: "cat",
                table: "layout",
                columns: new[] { "activo", "fecha_inicio_autorizada", "fecha_fin_autorizada" });

            migrationBuilder.CreateIndex(
                name: "IX_tareas_id_dato",
                schema: "carta",
                table: "tareas",
                column: "id_dato");

            migrationBuilder.CreateIndex(
                name: "IX_tareas_id_estado",
                schema: "carta",
                table: "tareas",
                column: "id_estado");

            migrationBuilder.CreateIndex(
                name: "IX_tipos_cartas_clave",
                schema: "carta",
                table: "tipos_cartas",
                column: "clave",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "archivos",
                schema: "carta");

            migrationBuilder.DropTable(
                name: "carta",
                schema: "bit");

            migrationBuilder.DropTable(
                name: "config",
                schema: "bit");

            migrationBuilder.DropTable(
                name: "configuracion",
                schema: "sist");

            migrationBuilder.DropTable(
                name: "estado",
                schema: "cat");

            migrationBuilder.DropTable(
                name: "firmante",
                schema: "cat");

            migrationBuilder.DropTable(
                name: "firmas",
                schema: "carta");

            migrationBuilder.DropTable(
                name: "folios_consecutivos",
                schema: "carta");

            migrationBuilder.DropTable(
                name: "layout",
                schema: "cat");

            migrationBuilder.DropTable(
                name: "tareas",
                schema: "carta");

            migrationBuilder.DropTable(
                name: "datos",
                schema: "carta");

            migrationBuilder.DropTable(
                name: "estado_tarea",
                schema: "cat");

            migrationBuilder.DropTable(
                name: "tipos_cartas",
                schema: "carta");
        }
    }
}
