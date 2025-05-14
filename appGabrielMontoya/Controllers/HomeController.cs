using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using appGabrielMontoya.Models;

namespace appGabrielMontoya.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Login");
        }
        return View();
    }

    public IActionResult Privacy()
    {
        if (!UsuarioAutenticado())
        {
            return RedirectToAction("Login", "Login");
        }
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
