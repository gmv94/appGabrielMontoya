using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using appGabrielMontoya.Data;
using appGabrielMontoya.Models;

namespace appGabrielMontoya.Controllers
{
    public class FondoMonetarioController : Controller
    {
        private readonly AppDBContext _appDBContext;

        public FondoMonetarioController(AppDBContext context)
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
            var fondos = await _appDBContext.FondosMonetarios
                .Where(f=> f.UsuarioId == usuarioId)
                .ToListAsync();
            return View(fondos);
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
        public async Task<IActionResult> Create(FondoMonetario fondo)
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }

            ModelState.Remove("Usuario");

            var usuarioId = ObtenerUsuarioActual();
            fondo.UsuarioId = usuarioId;

            if (ModelState.IsValid)
            {
                _appDBContext.Add(fondo);
                await _appDBContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(fondo);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }
            if (id == null) return NotFound();

            var fondo = await _appDBContext.FondosMonetarios.FindAsync(id);
            if (fondo == null) return NotFound();

            return View(fondo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FondoMonetario fondo)
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }
            if (id != fondo.FondoMonetarioId) return NotFound();

            var usuarioId = ObtenerUsuarioActual();
            fondo.UsuarioId = usuarioId;

            ModelState.Remove("Usuario");
            if (ModelState.IsValid)
            {
                _appDBContext.Update(fondo);
                await _appDBContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(fondo);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }
            if (id == null) return NotFound();

            var fondo = await _appDBContext.FondosMonetarios.FirstOrDefaultAsync(m => m.FondoMonetarioId == id);
            if (fondo == null) return NotFound();

            return View(fondo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }
            var fondo = await _appDBContext.FondosMonetarios.FindAsync(id);
            if (fondo != null)
            {
                _appDBContext.FondosMonetarios.Remove(fondo);
                await _appDBContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
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
    }
}
