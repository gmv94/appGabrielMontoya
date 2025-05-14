using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using appGabrielMontoya.Data;
using appGabrielMontoya.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace appGabrielMontoya.Controllers
{
    public class GastoEncabezadoController : Controller
    {
        private readonly AppDBContext _appDBContext;

        public GastoEncabezadoController(AppDBContext context)
        {
            _appDBContext = context;
        }

        public async Task<IActionResult> Index()
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }
            var gastos = await _appDBContext.GastosEncabezados
                .Include(g => g.FondoMonetario)
                .Include(g => g.GastoDetalles)
                .ToListAsync();

            return View(gastos);
        }

        public async Task<IActionResult> Create()
        {
            if (!UsuarioAutenticado())
                return RedirectToAction("Login", "Login");

            var usuarioId = ObtenerUsuarioActual();
            AsignarViewBagUsuario();

            await CargarFondosMonetariosAsync(usuarioId);
            await CargarTiposGastoAsync();

            return View(new GastoEncabezado
            {
                Fecha = DateTime.Now
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GastoEncabezado gasto, int TipoGastoId, decimal Monto)
        {
            if (!UsuarioAutenticado())
                return RedirectToAction("Login", "Login");

            gasto.UsuarioId = ObtenerUsuarioActual();

            if (gasto.Fecha == default || gasto.FondoMonetarioId == 0 || string.IsNullOrEmpty(gasto.NombreComercio) || Monto <= 0 || TipoGastoId == 0)
            {
                ViewBag.MensajeError = "Todos los campos son obligatorios.";
            }

            ModelState.Remove("Usuario");
            ModelState.Remove("FondoMonetario");
            if (!ModelState.IsValid)
            {
                await CargarFondosMonetariosAsync(gasto.UsuarioId);
                await CargarTiposGastoAsync();
                return View(gasto);
            }

            var presupuestos = await _appDBContext.Presupuestos
                .Where(p => p.UsuarioId == gasto.UsuarioId)
                .GroupBy(p => p.TipoGastoId)
                .Select(g => new
                {
                    TipoGastoId = g.Key,
                    TotalPresupuestado = g.Sum(x => x.Monto)
                }).ToListAsync();

            var gastosActuales = await _appDBContext.GastoDetalles
                .Include(g => g.GastoEncabezado)
                .Where(g => g.GastoEncabezado.UsuarioId == gasto.UsuarioId)
                .GroupBy(g => g.TipoGastoId)
                .Select(g => new
                {
                    TipoGastoId = g.Key,
                    TotalGastado = g.Sum(x => x.Monto)
                }).ToListAsync();

            var tipoPresupuesto = presupuestos.FirstOrDefault(p => p.TipoGastoId == TipoGastoId);
            var tipoGasto = gastosActuales.FirstOrDefault(g => g.TipoGastoId == TipoGastoId);

            if (tipoPresupuesto == null)
            {
                ViewBag.MensajeError = "No se encontró presupuesto para este tipo de gasto.";
                await CargarFondosMonetariosAsync(gasto.UsuarioId);
                await CargarTiposGastoAsync();
                AsignarViewBagUsuario();
                return View(gasto);
            }

            decimal totalPresupuestado = tipoPresupuesto?.TotalPresupuestado ?? 0;
            decimal totalGastado = tipoGasto?.TotalGastado ?? 0;
            decimal totalNuevo = totalGastado + Monto;

            if (totalNuevo > totalPresupuestado)
            {
                var mensajeError = "¡Atención! El presupuesto para este tipo de gasto está sobregirado. " +
                    $"Presupuestado: {totalPresupuestado:C}, Gastado: {totalGastado:C}, Nuevo Gasto: {Monto:C}";
                ViewBag.MensajeError = mensajeError;

                await CargarFondosMonetariosAsync(gasto.UsuarioId);
                await CargarTiposGastoAsync();
                return View(gasto);
            }

            using var transaction = await _appDBContext.Database.BeginTransactionAsync();
            try
            {
                _appDBContext.GastosEncabezados.Add(gasto);
                await _appDBContext.SaveChangesAsync();

                var detalle = new GastoDetalle
                {
                    GastoEncabezadoId = gasto.GastoEncabezadoId,
                    TipoGastoId = TipoGastoId,
                    Monto = Monto
                };

                _appDBContext.GastoDetalles.Add(detalle);
                await _appDBContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                await transaction.RollbackAsync();
                ViewBag.MensajeError = "Error al guardar el gasto.";
                await CargarFondosMonetariosAsync(gasto.UsuarioId);
                await CargarTiposGastoAsync();
                return View(gasto);
            }
        }

        private int ObtenerUsuarioActual()
        {
            try
            {
                var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
                if (int.TryParse(usuarioIdStr, out int usuarioId))
                {
                    return usuarioId;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private bool UsuarioAutenticado()
        {
            try
            {
                return HttpContext.Session.GetString("UsuarioNombre") != null;
            }
            catch
            {
                return false;
            }
        }

        private async Task CargarFondosMonetariosAsync(int usuarioId, int? fondoSeleccionado = null)
        {
            var fondos = await _appDBContext.FondosMonetarios
                .Where(f => f.UsuarioId == usuarioId)
                .ToListAsync();

            ViewBag.FondoMonetarioId = new SelectList(fondos, "FondoMonetarioId", "Nombre", fondoSeleccionado);
        }

        private async Task CargarFondosMonetariosYTiposGastoAsync(int usuarioId)
        {
            var fondos = await _appDBContext.FondosMonetarios
                .Where(f => f.UsuarioId == usuarioId)
                .ToListAsync();
            ViewBag.FondoMonetarioId = new SelectList(fondos, "FondoMonetarioId", "Nombre");

            var tipos = await _appDBContext.TiposGasto
                .Where(t => t.UsuarioId == usuarioId)
                .ToListAsync();
            ViewBag.TiposGasto = new SelectList(tipos, "TipoGastoId", "Nombre");
        }

        private async Task CargarTiposGastoAsync()
        {
            var usuarioId = ObtenerUsuarioActual();
            var tipos = await _appDBContext.TiposGasto
                .Where(t => t.UsuarioId == usuarioId)
                .ToListAsync();

            ViewBag.TiposGasto = new SelectList(tipos, "TipoGastoId", "Nombre");
        }

        private async Task<Usuario?> ObtenerUsuarioActualAsync()
        {
            try
            {
                var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
                if (int.TryParse(usuarioIdStr, out int usuarioId))
                {
                    return await _appDBContext.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == usuarioId);
                }
                return null;
            }
            catch
            {
                return null;
            }

        }

        private void AsignarViewBagUsuario()
        {
            var usuario = ObtenerUsuarioActualAsync();
            ViewBag.UsuarioNombre = $"{usuario.Result?.Nombre} {usuario.Result?.Apellido}";
        }
    }
}
