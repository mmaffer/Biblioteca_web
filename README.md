# Laboratorio 08 – MVC con Dapper (Biblioteca.Web)

## Cómo ejecutar
1. Ejecutar `Scripts/Script-Semana08.sql` en SSMS sobre `.\SQLEXPRESS` (usa `BibliotecaDB` de la semana 07; es re-ejecutable).
2. Abrir `lab8.slnx` en Visual Studio y ejecutar (o `dotnet run --project lab8`).
3. La cadena de conexión está en `lab8/appsettings.json` (`ConnectionStrings:BibliotecaDB`).

## Estructura
- `Models/` Libro (con DataAnnotations), Socio, PrestamoReporte, Autor
- `Repositorios/` LibroRepositorio, SocioRepositorio (Dapper + `CommandType.StoredProcedure`)
- `Controllers/` Libros, Socios, Prestamos (sin SQL ni conexiones)
- `Views/Libros|Socios|Prestamos`, parcial `Views/Libros/_FilaLibro.cshtml` (punto extra)

## Explicación: recorrido de una petición – `GET /Libros?buscar=casa`
1. **Ruta:** la ruta por defecto `{controller=Libros}/{action=Index}/{id?}` resuelve `LibrosController.Index`; `buscar` se enlaza desde la query string.
2. **Controlador:** `Index(string? buscar)` (async) llama a `_libros.BuscarAsync(buscar)`; si no hay texto llama a `ListarAsync()`. El `LibroRepositorio` llega por inyección en el constructor (registrado con `AddScoped` en `Program.cs`).
3. **Repositorio:** `BuscarAsync` abre un `SqlConnection` con la cadena leída de `IConfiguration` y ejecuta `QueryAsync<Libro>("usp_Libros_BuscarPorTitulo", new { Titulo = ... }, commandType: CommandType.StoredProcedure)`.
4. **Procedimiento almacenado:** `usp_Libros_BuscarPorTitulo` hace `Libros INNER JOIN Autores` con `Activo = 1` y `Titulo LIKE '%' + @Titulo + '%'`. Dapper mapea las columnas a `Libro` (incluye `AutorNombre`).
5. **Vista:** el controlador devuelve `View(lista)`; `Views/Libros/Index.cshtml` (`@model IEnumerable<Libro>`) pinta la tabla y por cada libro renderiza la parcial `_FilaLibro`.

**Cómo se pasan los datos y por qué**
- **Modelo (`@model`)**: el dato principal de la vista (la lista de libros / el libro del formulario); es fuertemente tipado y valida en compilación.
- **ViewData**: datos auxiliares y de vida corta en la misma petición: `Title`, el texto buscado (`Buscar`), la lista desplegable de autores (`Autores`) y el rango de fechas del reporte (`Desde`/`Hasta`).
- **TempData**: solo para el resultado de Create/Edit/Delete, porque tras guardar se hace `RedirectToAction` (Post/Redirect/Get) y ViewData se perdería al ser otra petición; TempData sobrevive a esa redirección y se muestra una sola vez en `_Layout`.

## Observaciones y conclusiones
- Separar controlador / repositorio / procedimiento almacenado deja el SQL en un solo lugar y los controladores simples y sin acoplarse a la base de datos.
- Dapper reduce el código de acceso a datos frente a ADO.NET puro, manteniendo el control del SQL.
- La eliminación lógica (`Activo = 0`) conserva el historial de préstamos y evita errores de llave foránea.
- Los duplicados (ISBN, DNI) se detectan en el procedimiento (devuelve -1) y se muestran con `ModelState.AddModelError` en lugar de dejar que la página falle.
- Pendiente del alumno: capturas de pantalla de las vistas y URL del repositorio.
