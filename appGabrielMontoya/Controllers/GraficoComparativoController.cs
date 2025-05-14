using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using appGabrielMontoya.Data;
using appGabrielMontoya.Models;

public class GraficoComparativoController : Controller
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly AppDBContext _appDBContext;

    public GraficoComparativoController(AppDBContext context)
    {
        _appDBContext = context;
    }

    public IActionResult Index()
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Login");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GenerarGrafico(DateTime fechaInicio, DateTime fechaFin)
    {
        try
        {
            var usuarioId = ObtenerUsuarioActual();
            if (usuarioId < 1)
            {
                return RedirectToAction("Login", "Login");
            }

            var presupuestos = await _appDBContext.Presupuestos
                .Where(p => p.UsuarioId == usuarioId &&
                           p.Anio >= fechaInicio.Year && p.Anio <= fechaFin.Year &&
                           ((p.Anio > fechaInicio.Year && p.Anio < fechaFin.Year) ||
                            (p.Anio == fechaInicio.Year && p.Mes >= fechaInicio.Month) ||
                            (p.Anio == fechaFin.Year && p.Mes <= fechaFin.Month)))
                .Include(p => p.TipoGasto)
                .ToListAsync();


            var gastosDetalle = await _appDBContext.GastoDetalles
                .Include(gd => gd.GastoEncabezado)
                .Where(gd =>
                    gd.GastoEncabezado.UsuarioId == usuarioId &&
                    gd.GastoEncabezado.Fecha >= fechaInicio &&
                    gd.GastoEncabezado.Fecha < fechaFin.AddDays(1))
                .Select(gd => new {
                    gd.Monto,
                    gd.GastoEncabezado.Fecha,
                    gd.TipoGastoId
                })
                .ToListAsync();


            var datosGrafico = new List<DatosGrafico>();

            foreach (var tipoGasto in await _appDBContext.TiposGasto.ToListAsync())
            {
                var presupuestoTipo = presupuestos
                    .Where(p => p.TipoGastoId == tipoGasto.TipoGastoId)
                    .Sum(p => p.Monto);

                var gastoTipo = gastosDetalle
                    .Where(g => g.TipoGastoId == tipoGasto.TipoGastoId)
                    .Sum(g => g.Monto);

                if (presupuestoTipo > 0 || gastoTipo > 0)
                {
                    datosGrafico.Add(new DatosGrafico
                    {
                        TipoGasto = tipoGasto.Nombre,
                        Presupuestado = presupuestoTipo,
                        Ejecutado = gastoTipo
                    });
                }
            }

            return View("Index", datosGrafico);
        }
        catch (Exception)
        {
            return View();
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
}