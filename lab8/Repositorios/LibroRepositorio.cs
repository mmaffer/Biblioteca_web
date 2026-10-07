using System.Data;
using Biblioteca.Web.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Biblioteca.Web.Repositorios;

public class LibroRepositorio
{
    private readonly string _cadena;

    public LibroRepositorio(IConfiguration config)
    {
        _cadena = config.GetConnectionString("BibliotecaDB")
            ?? throw new InvalidOperationException("Falta la cadena de conexión 'BibliotecaDB' en appsettings.json.");
    }

    public async Task<IEnumerable<Libro>> ListarAsync()
    {
        using var cn = new SqlConnection(_cadena);
        return await cn.QueryAsync<Libro>("usp_Libros_ListarActivos", commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Libro>> BuscarAsync(string titulo)
    {
        using var cn = new SqlConnection(_cadena);
        return await cn.QueryAsync<Libro>("usp_Libros_BuscarPorTitulo",
            new { Titulo = titulo }, commandType: CommandType.StoredProcedure);
    }

    public async Task<Libro?> ObtenerAsync(int id)
    {
        using var cn = new SqlConnection(_cadena);
        return await cn.QueryFirstOrDefaultAsync<Libro>("usp_Libros_ObtenerPorId",
            new { LibroId = id }, commandType: CommandType.StoredProcedure);
    }

    /// <returns>Id nuevo, o -1 si el ISBN ya existe.</returns>
    public async Task<int> InsertarAsync(Libro l)
    {
        using var cn = new SqlConnection(_cadena);
        return await cn.QuerySingleAsync<int>("usp_Libros_Insertar",
            new { l.Titulo, l.ISBN, l.AutorId, l.Ejemplares }, commandType: CommandType.StoredProcedure);
    }

    /// <returns>Filas afectadas (0 = no existe), o -1 si el ISBN es de otro libro.</returns>
    public async Task<int> ActualizarAsync(Libro l)
    {
        using var cn = new SqlConnection(_cadena);
        return await cn.QuerySingleAsync<int>("usp_Libros_Actualizar",
            new { l.LibroId, l.Titulo, l.ISBN, l.AutorId, l.Ejemplares }, commandType: CommandType.StoredProcedure);
    }

    /// <summary>Eliminación lógica (Activo = 0).</summary>
    public async Task<int> EliminarAsync(int id)
    {
        using var cn = new SqlConnection(_cadena);
        return await cn.QuerySingleAsync<int>("usp_Libros_Eliminar",
            new { LibroId = id }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Autor>> ListarAutoresAsync()
    {
        using var cn = new SqlConnection(_cadena);
        return await cn.QueryAsync<Autor>("usp_Autores_ListarActivos", commandType: CommandType.StoredProcedure);
    }
}
