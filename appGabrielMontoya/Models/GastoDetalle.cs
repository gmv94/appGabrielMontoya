namespace appGabrielMontoya.Models
{
    public class GastoDetalle
    {
        public int GastoDetalleId { get; set; }
        public int GastoEncabezadoId { get; set; }
        public GastoEncabezado GastoEncabezado { get; set; } = null!;
        public int TipoGastoId { get; set; }
        public TipoGasto TipoGasto { get; set; } = null!;
        public decimal Monto { get; set; }
    }
}
