using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace appGabrielMontoya.Migrations
{
    /// <inheritdoc />
    public partial class adicionaratributoUsuarioIdenTipoGastos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "TiposGasto",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "TiposGasto");
        }
    }
}
