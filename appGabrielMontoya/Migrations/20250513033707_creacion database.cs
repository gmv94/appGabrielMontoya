using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace appGabrielMontoya.Migrations
{
    /// <inheritdoc />
    public partial class creaciondatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TiposGasto",
                columns: table => new
                {
                    TipoGastoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposGasto", x => x.TipoGastoId);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identificacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Login = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioId);
                });

            migrationBuilder.CreateTable(
                name: "FondosMonetarios",
                columns: table => new
                {
                    FondoMonetarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FondosMonetarios", x => x.FondoMonetarioId);
                    table.ForeignKey(
                        name: "FK_FondosMonetarios_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Presupuestos",
                columns: table => new
                {
                    PresupuestoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    TipoGastoId = table.Column<int>(type: "int", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presupuestos", x => x.PresupuestoId);
                    table.ForeignKey(
                        name: "FK_Presupuestos_TiposGasto_TipoGastoId",
                        column: x => x.TipoGastoId,
                        principalTable: "TiposGasto",
                        principalColumn: "TipoGastoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Presupuestos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Depositos",
                columns: table => new
                {
                    DepositoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FondoMonetarioId = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Depositos", x => x.DepositoId);
                    table.ForeignKey(
                        name: "FK_Depositos_FondosMonetarios_FondoMonetarioId",
                        column: x => x.FondoMonetarioId,
                        principalTable: "FondosMonetarios",
                        principalColumn: "FondoMonetarioId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Depositos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GastosEncabezados",
                columns: table => new
                {
                    GastoEncabezadoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FondoMonetarioId = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreComercio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoDocumento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GastosEncabezados", x => x.GastoEncabezadoId);
                    table.ForeignKey(
                        name: "FK_GastosEncabezados_FondosMonetarios_FondoMonetarioId",
                        column: x => x.FondoMonetarioId,
                        principalTable: "FondosMonetarios",
                        principalColumn: "FondoMonetarioId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GastosEncabezados_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GastoDetalles",
                columns: table => new
                {
                    GastoDetalleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GastoEncabezadoId = table.Column<int>(type: "int", nullable: false),
                    TipoGastoId = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GastoDetalles", x => x.GastoDetalleId);
                    table.ForeignKey(
                        name: "FK_GastoDetalles_GastosEncabezados_GastoEncabezadoId",
                        column: x => x.GastoEncabezadoId,
                        principalTable: "GastosEncabezados",
                        principalColumn: "GastoEncabezadoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GastoDetalles_TiposGasto_TipoGastoId",
                        column: x => x.TipoGastoId,
                        principalTable: "TiposGasto",
                        principalColumn: "TipoGastoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Depositos_FondoMonetarioId",
                table: "Depositos",
                column: "FondoMonetarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Depositos_UsuarioId",
                table: "Depositos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_FondosMonetarios_UsuarioId",
                table: "FondosMonetarios",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_GastoDetalles_GastoEncabezadoId",
                table: "GastoDetalles",
                column: "GastoEncabezadoId");

            migrationBuilder.CreateIndex(
                name: "IX_GastoDetalles_TipoGastoId",
                table: "GastoDetalles",
                column: "TipoGastoId");

            migrationBuilder.CreateIndex(
                name: "IX_GastosEncabezados_FondoMonetarioId",
                table: "GastosEncabezados",
                column: "FondoMonetarioId");

            migrationBuilder.CreateIndex(
                name: "IX_GastosEncabezados_UsuarioId",
                table: "GastosEncabezados",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Presupuestos_TipoGastoId",
                table: "Presupuestos",
                column: "TipoGastoId");

            migrationBuilder.CreateIndex(
                name: "IX_Presupuestos_UsuarioId_TipoGastoId_Mes_Anio",
                table: "Presupuestos",
                columns: new[] { "UsuarioId", "TipoGastoId", "Mes", "Anio" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Depositos");

            migrationBuilder.DropTable(
                name: "GastoDetalles");

            migrationBuilder.DropTable(
                name: "Presupuestos");

            migrationBuilder.DropTable(
                name: "GastosEncabezados");

            migrationBuilder.DropTable(
                name: "TiposGasto");

            migrationBuilder.DropTable(
                name: "FondosMonetarios");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
