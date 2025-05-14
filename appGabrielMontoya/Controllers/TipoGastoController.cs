using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using appGabrielMontoya.Data;
using appGabrielMontoya.Models;

namespace appGabrielMontoya.Controllers
{
    public class TipoGastoController : Controller
    {
        private readonly AppDBContext _appDBContext;

        public TipoGastoController(AppDBContext context)
        {
            _appDBContext = context;
        }

        public async Task<IActionResult> Index()
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }
            var usuarioId = ObtenerUsuarioActual();
            var tipos = await _appDBContext.TiposGasto
                .Where(t=> t.UsuarioId == usuarioId).ToListAsync();
            return View(tipos);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }
            if (id == null) return NotFound();
            var tipo = await _appDBContext.TiposGasto
                .FirstOrDefaultAsync(m => m.TipoGastoId == id);

            if (tipo == null) return NotFound();

            return View(tipo);
        }

        public IActionResult Create()
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoGasto tipoGasto)
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }
            var usuarioId = ObtenerUsuarioActual();
            if (ModelState.IsValid)
            {
                tipoGasto.UsuarioId = usuarioId;
                _appDBContext.Add(tipoGasto);
                await _appDBContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(tipoGasto);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }
            if (id == null) return NotFound();

            var tipo = await _appDBContext.TiposGasto.FindAsync(id);
            if (tipo == null) return NotFound();

            return View(tipo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TipoGasto tipoGasto)
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }
            var usuarioId = ObtenerUsuarioActual();
            tipoGasto.UsuarioId = usuarioId;
            if (id != tipoGasto.TipoGastoId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _appDBContext.Update(tipoGasto);
                    await _appDBContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoGastoExists(tipoGasto.TipoGastoId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(tipoGasto);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }
            if (id == null) return NotFound();

            var tipo = await _appDBContext.TiposGasto
                .FirstOrDefaultAsync(m => m.TipoGastoId == id);

            if (tipo == null) return NotFound();

            return View(tipo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }
            var tipo = await _appDBContext.TiposGasto.FindAsync(id);
            if (tipo != null)
            {
                _appDBContext.TiposGasto.Remove(tipo);
                await _appDBContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TipoGastoExists(int id)
        {
            return _appDBContext.TiposGasto.Any(e => e.TipoGastoId == id);
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
