using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace appGabrielMontoya.Migrations
{
    /// <inheritdoc />
    public partial class CorrecciontablaFondoMonetario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Presupuestos_TiposGasto_TipoGastoId",
                table: "Presupuestos");

            migrationBuilder.AddForeignKey(
                name: "FK_Presupuestos_TiposGasto_TipoGastoId",
                table: "Presupuestos",
                column: "TipoGastoId",
                principalTable: "TiposGasto",
                principalColumn: "TipoGastoId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Presupuestos_TiposGasto_TipoGastoId",
                table: "Presupuestos");

            migrationBuilder.AddForeignKey(
                name: "FK_Presupuestos_TiposGasto_TipoGastoId",
                table: "Presupuestos",
                column: "TipoGastoId",
                principalTable: "TiposGasto",
                principalColumn: "TipoGastoId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
