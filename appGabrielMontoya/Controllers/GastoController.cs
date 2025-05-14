using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using appGabrielMontoya.Data;
using appGabrielMontoya.Models;

namespace appGabrielMontoya.Controllers
{
    public class GastoController : Controller
    {
        private readonly AppDBContext _appDBContext;

        public GastoController(AppDBContext context)
        {
            _appDBContext = context;
        }

        public async Task<IActionResult> Index()
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }
            var usuario = await ObtenerUsuarioActualAsync();
            if (usuario == null) return RedirectToAction("Login", "Login");

            var gastos = await _appDBContext.GastosEncabezados
                .Include(g => g.FondoMonetario)
                .Include(g => g.GastoDetalles)
                .ThenInclude(d => d.TipoGasto)
                .Where(g => g.UsuarioId == usuario.UsuarioId)
                .ToListAsync();

            return View(gastos);
        }

        public IActionResult Create()
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }
            ViewData["Fondos"] = _appDBContext.FondosMonetarios.ToList();
            ViewData["TiposGasto"] = _appDBContext.TiposGasto.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(GastoEncabezado encabezado, List<GastoDetalle> detalles)
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }
            var usuario = await ObtenerUsuarioActualAsync();
            if (usuario == null) return RedirectToAction("Login", "Login");

            if (detalles == null || detalles.Count == 0)
            {
                ModelState.AddModelError("", "Debe ingresar al menos un detalle de gasto.");
                return View(encabezado);
            }

            using var transaction = await _appDBContext.Database.BeginTransactionAsync();
            try
            {
                encabezado.UsuarioId = usuario.UsuarioId;
                encabezado.Fecha = DateTime.Now;

                _appDBContext.GastosEncabezados.Add(encabezado);
                await _appDBContext.SaveChangesAsync();

                decimal montoTotal = 0;
                List<string> alertas = new();

                foreach (var detalle in detalles)
                {
                    detalle.GastoEncabezadoId = encabezado.GastoEncabezadoId;
                    _appDBContext.GastoDetalles.Add(detalle);
                    montoTotal += detalle.Monto;

                    var presupuesto = await _appDBContext.Presupuestos.FirstOrDefaultAsync(p =>
                        p.UsuarioId == usuario.UsuarioId &&
                        p.TipoGastoId == detalle.TipoGastoId &&
                        p.Mes == encabezado.Fecha.Month &&
                        p.Anio == encabezado.Fecha.Year);

                    var totalGastado = await _appDBContext.GastoDetalles
                        .Include(d => d.GastoEncabezado)
                        .Where(d =>
                            d.TipoGastoId == detalle.TipoGastoId &&
                            d.GastoEncabezado.UsuarioId == usuario.UsuarioId &&
                            d.GastoEncabezado.Fecha.Month == encabezado.Fecha.Month &&
                            d.GastoEncabezado.Fecha.Year == encabezado.Fecha.Year)
                        .SumAsync(d => d.Monto);

                    if (presupuesto != null && totalGastado + detalle.Monto > presupuesto.Monto)
                    {
                        decimal sobregiro = (totalGastado + detalle.Monto) - presupuesto.Monto;
                        alertas.Add($"Tipo de Gasto: {detalle.TipoGasto.Nombre} — Presupuesto: {presupuesto.Monto:C} — Sobregirado en: {sobregiro:C}");
                    }
                }

                await _appDBContext.SaveChangesAsync();
                await transaction.CommitAsync();

                if (alertas.Any())
                {
                    TempData["Alerta"] = "Atención: Algunos gastos sobrepasan el presupuesto:\n" + string.Join("\n", alertas);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", "Error al guardar el gasto: " + ex.Message);
                return View(encabezado);
            }
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
    }
}
