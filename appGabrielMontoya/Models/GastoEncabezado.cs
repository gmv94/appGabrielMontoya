namespace appGabrielMontoya.Models
{
    public class GastoEncabezado
    {
        public int GastoEncabezadoId { get; set; }
        public DateTime Fecha { get; set; }
        public int FondoMonetarioId { get; set; }
        public FondoMonetario FondoMonetario { get; set; } = null!;
        public string Observaciones { get; set; } = null!;
        public string NombreComercio { get; set; } = null!;
        public string TipoDocumento { get; set; } = null!; // Comprobante, Factura, Otro
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public ICollection<GastoDetalle> GastoDetalles { get; set; } = new List<GastoDetalle>();
    }

}
