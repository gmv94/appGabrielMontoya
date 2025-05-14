namespace appGabrielMontoya.Models
{
    public class Deposito
    {
        public int DepositoId { get; set; }
        public DateTime Fecha { get; set; }
        public int FondoMonetarioId { get; set; }
        public FondoMonetario FondoMonetario { get; set; } = null!;
        public decimal Monto { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}
