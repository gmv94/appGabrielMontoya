using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using appGabrielMontoya.Data;
using appGabrielMontoya.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace appGabrielMontoya.Controllers
{
    public class DepositoController : Controller
    {
        private readonly AppDBContext _appDBContext;

        public DepositoController(AppDBContext context)
        {
            _appDBContext = context;
        }

        public async Task<IActionResult> Index()
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }
            var depositos = await _appDBContext.Depositos
                .Include(d => d.FondoMonetario)
                .Include(d => d.Usuario)
                .ToListAsync();

            return View(depositos);
        }

        public IActionResult Create()
        {
            CargarFondosMonetariosAsync().Wait();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Deposito deposito)
        {
            if (!UsuarioAutenticado())
            {
                return RedirectToAction("Login", "Login");
            }

            await CargarFondosMonetariosAsync();

            ModelState.Remove("FondoMonetario");
            ModelState.Remove("Usuario");
            if (ModelState.IsValid)
            {
                deposito.UsuarioId = ObtenerUsuarioActual();
                _appDBContext.Add(deposito);
                await _appDBContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(deposito);
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

        private async Task CargarFondosMonetariosAsync()
        {
            var usuarioId = ObtenerUsuarioActual();
            var fondos = await _appDBContext.FondosMonetarios
                .Where(t => t.UsuarioId == usuarioId)
                .ToListAsync();

            ViewBag.FondoMonetarioId = new SelectList(fondos, "FondoMonetarioId", "Nombre");
        }
    }
}
