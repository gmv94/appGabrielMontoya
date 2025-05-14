using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using appGabrielMontoya.Data;
using appGabrielMontoya.Models;
using System;

namespace appGabrielMontoya.Controllers
{
    public class PresupuestoController : Controller
    {
        private readonly AppDBContext _appDBContext;

        public PresupuestoController(AppDBContext context)
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

            var presupuestos = await _appDBContext.Presupuestos
                .Include(p => p.TipoGasto)
                .Where(p => p.UsuarioId == usuario.UsuarioId)
                .ToListAsync();

            return View(presupuestos);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var usuario = await ObtenerUsuarioActualAsync();
            if (usuario == null) return RedirectToAction("Login", "Login");

            var presupuesto = await _appDBContext.Presupuestos
                .Include(p => p.TipoGasto)
                .FirstOrDefaultAsync(p => p.PresupuestoId == id && p.UsuarioId == usuario.UsuarioId);

            if (presupuesto == null) return NotFound();

            return View(presupuesto);
        }

        public IActionResult Create()
        {


            var usuario = ObtenerUsuarioActual();
            if (usuario == 0) return RedirectToAction("Login", "Login");
            var gastos = _appDBContext.TiposGasto
                .Where(g => g.UsuarioId == usuario)
                .ToList();
            ViewData["TipoGastoId"] = new SelectList(gastos, "TipoGastoId", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Presupuesto presupuesto)
        {
            var usuario = await ObtenerUsuarioActualAsync();
            if (usuario == null) return RedirectToAction("Login", "Login");

            presupuesto.UsuarioId = usuario.UsuarioId;

            if (presupuesto.Monto <= 0)
                ModelState.AddModelError("Monto", "El monto debe ser mayor que cero.");

            var existe = await _appDBContext.Presupuestos.AnyAsync(p =>
                p.UsuarioId == presupuesto.UsuarioId &&
                p.TipoGastoId == presupuesto.TipoGastoId &&
                p.Mes == presupuesto.Mes &&
                p.Anio == presupuesto.Anio);

            if (existe)
                ModelState.AddModelError("", "Ya existe un presupuesto registrado para este mes, año y tipo de gasto.");

            ModelState.Remove("Usuario");
            ModelState.Remove("TipoGasto");

            if (ModelState.IsValid)
            {
                _appDBContext.Add(presupuesto);
                await _appDBContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(presupuesto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var usuario = await ObtenerUsuarioActualAsync();
            if (usuario == null) return RedirectToAction("Login", "Login");

            var presupuesto = await _appDBContext.Presupuestos
                .FirstOrDefaultAsync(p => p.PresupuestoId == id && p.UsuarioId == usuario.UsuarioId);

            if (presupuesto == null) return NotFound();

            ViewData["TipoGastoId"] = new SelectList(_appDBContext.TiposGasto, "TipoGastoId", "Nombre", presupuesto.TipoGastoId);
            return View(presupuesto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Presupuesto presupuesto)
        {
            if (id != presupuesto.PresupuestoId) return NotFound();

            var usuario = await ObtenerUsuarioActualAsync();
            if (usuario == null) return RedirectToAction("Login", "Login");

            presupuesto.UsuarioId = usuario.UsuarioId;

            if (presupuesto.Monto <= 0)
                ModelState.AddModelError("Monto", "El monto debe ser mayor que cero.");

            var existeDuplicado = await _appDBContext.Presupuestos.AnyAsync(p =>
                p.PresupuestoId != presupuesto.PresupuestoId &&
                p.UsuarioId == presupuesto.UsuarioId &&
                p.TipoGastoId == presupuesto.TipoGastoId &&
                p.Mes == presupuesto.Mes &&
                p.Anio == presupuesto.Anio);

            if (existeDuplicado)
                ModelState.AddModelError("", "Ya existe otro presupuesto con los mismos datos.");

            if (ModelState.IsValid)
            {
                try
                {
                    _appDBContext.Update(presupuesto);
                    await _appDBContext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PresupuestoExists(presupuesto.PresupuestoId))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewData["TipoGastoId"] = new SelectList(_appDBContext.TiposGasto, "TipoGastoId", "Nombre", presupuesto.TipoGastoId);
            return View(presupuesto);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var usuario = await ObtenerUsuarioActualAsync();
            if (usuario == null) return RedirectToAction("Login", "Login");

            var presupuesto = await _appDBContext.Presupuestos
                .Include(p => p.TipoGasto)
                .FirstOrDefaultAsync(p => p.PresupuestoId == id && p.UsuarioId == usuario.UsuarioId);

            if (presupuesto == null) return NotFound();

            return View(presupuesto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await ObtenerUsuarioActualAsync();
            if (usuario == null) return RedirectToAction("Login", "Login");

            var presupuesto = await _appDBContext.Presupuestos
                .FirstOrDefaultAsync(p => p.PresupuestoId == id && p.UsuarioId == usuario.UsuarioId);

            if (presupuesto != null)
            {
                _appDBContext.Presupuestos.Remove(presupuesto);
                await _appDBContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PresupuestoExists(int id)
        {
            return _appDBContext.Presupuestos.Any(p => p.PresupuestoId == id);
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
}
