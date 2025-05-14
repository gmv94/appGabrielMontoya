namespace appGabrielMontoya.Models
{
    public class FondoMonetario
    {
        public int FondoMonetarioId { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public ICollection<GastoEncabezado> GastosEncabezados { get; set; } = new List<GastoEncabezado>();
        public ICollection<Deposito> Depositos { get; set; } = new List<Deposito>();
    }
}
