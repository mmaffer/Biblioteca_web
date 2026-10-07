using Biblioteca.Web.Models;
using Biblioteca.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Web.Controllers;

public class PrestamosController : Controller
{
    private readonly SocioRepositorio _repo;

    public PrestamosController(SocioRepositorio repo) => _repo = repo;

    // GET /Prestamos/Reporte?desde=2026-09-01&hasta=2026-09-30
    public async Task<IActionResult> Reporte(DateTime? desde, DateTime? hasta)
    {
        var d = desde ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
        var h = hasta ?? DateTime.Today;

        ViewData["Desde"] = d.ToString("yyyy-MM-dd");
        ViewData["Hasta"] = h.ToString("yyyy-MM-dd");

        if (d > h)
        {
            ViewData["Error"] = "La fecha «desde» no puede ser mayor que «hasta».";
            return View(Enumerable.Empty<PrestamoReporte>());
        }
        return View(await _repo.ReporteAsync(d, h));
    }
}
