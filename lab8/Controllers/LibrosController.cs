using Biblioteca.Web.Models;
using Biblioteca.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Biblioteca.Web.Controllers;

public class LibrosController : Controller
{
    private readonly LibroRepositorio _libros;

    public LibrosController(LibroRepositorio libros) => _libros = libros;

    public async Task<IActionResult> Index(string? buscar)
    {
        ViewData["Buscar"] = buscar;
        var lista = string.IsNullOrWhiteSpace(buscar)
            ? await _libros.ListarAsync()
            : await _libros.BuscarAsync(buscar.Trim());
        return View(lista);
    }

    public async Task<IActionResult> Details(int id)
    {
        var libro = await _libros.ObtenerAsync(id);
        if (libro is null) return NotFound();
        return View(libro);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await CargarAutoresAsync();
        return View(new Libro { Ejemplares = 1 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Libro libro)
    {
        if (ModelState.IsValid)
        {
            var id = await _libros.InsertarAsync(libro);
            if (id == -1)
                ModelState.AddModelError(nameof(Libro.ISBN), "Ya existe un libro con ese ISBN.");
            else
            {
                TempData["Mensaje"] = $"Libro «{libro.Titulo}» registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
        }
        await CargarAutoresAsync(libro.AutorId);
        return View(libro);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var libro = await _libros.ObtenerAsync(id);
        if (libro is null) return NotFound();
        await CargarAutoresAsync(libro.AutorId);
        return View(libro);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Libro libro)
    {
        if (id != libro.LibroId) return BadRequest();
        if (ModelState.IsValid)
        {
            var r = await _libros.ActualizarAsync(libro);
            if (r == -1)
                ModelState.AddModelError(nameof(Libro.ISBN), "Ese ISBN pertenece a otro libro.");
            else if (r == 0)
                return NotFound();
            else
            {
                TempData["Mensaje"] = $"Libro «{libro.Titulo}» actualizado.";
                return RedirectToAction(nameof(Index));
            }
        }
        await CargarAutoresAsync(libro.AutorId);
        return View(libro);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var libro = await _libros.ObtenerAsync(id);
        if (libro is null) return NotFound();
        return View(libro);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var filas = await _libros.EliminarAsync(id);
        if (filas > 0) TempData["Mensaje"] = "Libro eliminado (baja lógica).";
        else TempData["Error"] = "El libro no existe o ya estaba eliminado.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarAutoresAsync(int? seleccionado = null)
    {
        var autores = await _libros.ListarAutoresAsync();
        ViewData["Autores"] = new SelectList(autores, nameof(Autor.AutorId), nameof(Autor.Nombre), seleccionado);
    }
}
