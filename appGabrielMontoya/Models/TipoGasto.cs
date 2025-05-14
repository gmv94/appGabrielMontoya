namespace appGabrielMontoya.Models
{
    public class TipoGasto
    {
        public int TipoGastoId { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public int UsuarioId { get; set; }

        public ICollection<Presupuesto> Presupuestos { get; set; } = new List<Presupuesto>();
        public ICollection<GastoDetalle> GastoDetalles { get; set; } = new List<GastoDetalle>();
    }

}
