namespace appGabrielMontoya.Models
{
    public class Presupuesto
    {
        public int PresupuestoId { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public int TipoGastoId { get; set; }
        public TipoGasto TipoGasto { get; set; } = null!;
        public int Mes { get; set; }
        public int Anio { get; set; }
        public decimal Monto { get; set; }
    }
}
