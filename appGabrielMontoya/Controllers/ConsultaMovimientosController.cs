using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using appGabrielMontoya.Data;

namespace appGabrielMontoya.Controllers
{
    public class ConsultaMovimientosController : Controller
    {
        private readonly AppDBContext _appDBContext;

        public ConsultaMovimientosController(AppDBContext context)
        {
            _appDBContext = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime fechaInicio, DateTime fechaFin)
        {
            var fechaFinAjustada = fechaFin.Date.AddDays(1);

            var gastos = await _appDBContext.GastoDetalles
                .Include(g => g.GastoEncabezado)
                    .ThenInclude(e => e.Usuario)
                .Include(g => g.GastoEncabezado)
                    .ThenInclude(e => e.FondoMonetario)
                .Include(g => g.TipoGasto)
                .Where(g => g.GastoEncabezado.Fecha >= fechaInicio.Date &&
                            g.GastoEncabezado.Fecha < fechaFinAjustada)
                .Select(g => new Movimiento
                {
                    TipoMovimiento = "Gasto",
                    Fecha = g.GastoEncabezado.Fecha,
                    Usuario = g.GastoEncabezado.Usuario.Nombre + " " + g.GastoEncabezado.Usuario.Apellido,
                    Fondo = g.GastoEncabezado.FondoMonetario.Nombre,
                    Comercio = g.GastoEncabezado.NombreComercio,
                    TipoDocumento = g.GastoEncabezado.TipoDocumento,
                    TipoGasto = g.TipoGasto.Nombre,
                    Monto = g.Monto
                })
                .ToListAsync();

            var depositos = await _appDBContext.Depositos
                .Include(d => d.Usuario)
                .Include(d => d.FondoMonetario)
                .Where(d => d.Fecha >= fechaInicio.Date && d.Fecha < fechaFinAjustada)
                .Select(d => new Movimiento
                {
                    TipoMovimiento = "Depósito",
                    Fecha = d.Fecha,
                    Usuario = d.Usuario.Nombre + " " + d.Usuario.Apellido,
                    Fondo = d.FondoMonetario.Nombre,
                    Comercio = "-",
                    TipoDocumento = "-",
                    TipoGasto = "-",
                    Monto = d.Monto
                })
                .ToListAsync();

            var movimientos = gastos.Concat(depositos)
                .OrderBy(m => m.Fecha)
                .ToList();

            ViewBag.FechaInicio = fechaInicio.ToShortDateString();
            ViewBag.FechaFin = fechaFin.ToShortDateString();

            return View(movimientos);
        }




        public class Movimiento
        {
            public string TipoMovimiento { get; set; }
            public DateTime Fecha { get; set; }
            public string Usuario { get; set; }
            public string Fondo { get; set; }
            public string Comercio { get; set; }
            public string TipoDocumento { get; set; }
            public string TipoGasto { get; set; }
            public decimal Monto { get; set; }
        }
    }

}
