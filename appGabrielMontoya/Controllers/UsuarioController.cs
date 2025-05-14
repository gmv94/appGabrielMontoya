using Microsoft.AspNetCore.Mvc;
using appGabrielMontoya.Data;
using appGabrielMontoya.Models;
using Microsoft.EntityFrameworkCore;

namespace appGabrielMontoya.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly AppDBContext _appDBContext;

        public UsuarioController(AppDBContext appDBContext)
        {
            _appDBContext = appDBContext;
        }

        public async Task<IActionResult> Index()
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }
            var usuarios = await _appDBContext.Usuarios.ToListAsync();
            return View(usuarios);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }
            if (id == null) return NotFound();

            var usuario = await _appDBContext.Usuarios
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null) return NotFound();

            return View(usuario);
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
        public async Task<IActionResult> Create(Usuario usuario)
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }

            bool loginExiste = await _appDBContext.Usuarios
                .AnyAsync(u => u.Login == usuario.Login);

            bool identificacionExiste = await _appDBContext.Usuarios
                .AnyAsync(u => u.Identificacion == usuario.Identificacion);

            if (loginExiste)
            {
                ModelState.AddModelError("Login", "Ya existe un usuario con este Login.");
            }

            if (identificacionExiste)
            {
                ModelState.AddModelError("Identificacion", "Ya existe un usuario con esta Identificación.");
            }

            if (ModelState.IsValid)
            {
                _appDBContext.Add(usuario);
                await _appDBContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }
            if (id == null) return NotFound();

            var usuario = await _appDBContext.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario usuario)
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }
            if (id != usuario.UsuarioId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _appDBContext.Update(usuario);
                    await _appDBContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.UsuarioId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }
            if (id == null) return NotFound();

            var usuario = await _appDBContext.Usuarios
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([Bind("UsuarioId")] Usuario usuarioInput)
        {
            var usuario = await _appDBContext.Usuarios.FindAsync(usuarioInput.UsuarioId);
            if (usuario != null)
            {
                _appDBContext.Usuarios.Remove(usuario);
                await _appDBContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _appDBContext.Usuarios.Any(e => e.UsuarioId == id);
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