using Microsoft.AspNetCore.Mvc;
using appGabrielMontoya.Data;
using appGabrielMontoya.Models;
using Microsoft.EntityFrameworkCore;

namespace appGabrielMontoya.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDBContext _appDBContext;

        public LoginController(AppDBContext context)
        {
            _appDBContext = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string login, string password)
        {
            var usuario = await _appDBContext.Usuarios
                .FirstOrDefaultAsync(u => u.Login == login && u.Password == password);

            if (usuario != null)
            {
                HttpContext.Session.SetString("UsuarioId", usuario.UsuarioId.ToString());
                HttpContext.Session.SetString("UsuarioNombre", $"{usuario.Nombre} {usuario.Apellido}");
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Mensaje = "Usuario o contraseña incorrectos";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Usuario usuario)
        {
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
                return RedirectToAction("Login");
            }

            return View(usuario);
        }
    }
}
