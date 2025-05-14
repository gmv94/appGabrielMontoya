namespace appGabrielMontoya.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string Identificacion { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public DateTime FechaNacimiento { get; set; }
        public string Direccion { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Telefono { get; set; } = null!;

        public ICollection<FondoMonetario> FondosMonetarios { get; set; } = new List<FondoMonetario>();
        public ICollection<Presupuesto> Presupuestos { get; set; } = new List<Presupuesto>();
        public ICollection<GastoEncabezado> GastosEncabezados { get; set; } = new List<GastoEncabezado>();
        public ICollection<Deposito> Depositos { get; set; } = new List<Deposito>();
    }
}
