using System.Data;
using Biblioteca.Web.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Biblioteca.Web.Repositorios;

public class SocioRepositorio
{
    private readonly string _cadena;

    public SocioRepositorio(IConfiguration config)
    {
        _cadena = config.GetConnectionString("BibliotecaDB")
            ?? throw new InvalidOperationException("Falta la cadena de conexión 'BibliotecaDB' en appsettings.json.");
    }

    public async Task<IEnumerable<Socio>> ListarAsync()
    {
        using var cn = new SqlConnection(_cadena);
        return await cn.QueryAsync<Socio>("usp_Socios_ListarActivos", commandType: CommandType.StoredProcedure);
    }

    /// <returns>Id nuevo, o -1 si el DNI ya existe.</returns>
    public async Task<int> InsertarAsync(Socio s)
    {
        using var cn = new SqlConnection(_cadena);
        return await cn.QuerySingleAsync<int>("usp_Socios_Insertar",
            new { s.DNI, s.Nombre, s.Email }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<PrestamoReporte>> ReporteAsync(DateTime desde, DateTime hasta)
    {
        using var cn = new SqlConnection(_cadena);
        return await cn.QueryAsync<PrestamoReporte>("usp_Prestamos_Reporte",
            new { Desde = desde.Date, Hasta = hasta.Date }, commandType: CommandType.StoredProcedure);
    }
}
