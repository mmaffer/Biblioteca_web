using Biblioteca.Web.Models;
using Biblioteca.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Web.Controllers;

public class SociosController : Controller
{
    private readonly SocioRepositorio _socios;

    public SociosController(SocioRepositorio socios) => _socios = socios;

    public async Task<IActionResult> Index()
    {
        return View(await _socios.ListarAsync());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new Socio());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Socio socio)
    {
        if (ModelState.IsValid)
        {
            var id = await _socios.InsertarAsync(socio);
            if (id == -1)
                ModelState.AddModelError(nameof(Socio.DNI), "Ya existe un socio registrado con ese DNI.");
            else
            {
                TempData["Mensaje"] = $"Socio «{socio.Nombre}» registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
        }
        return View(socio);
    }
}
