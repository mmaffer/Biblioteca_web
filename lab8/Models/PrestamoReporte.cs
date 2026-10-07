using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Web.Models;

// Una fila por libro prestado (resultado del INNER JOIN de 4 tablas).
public class PrestamoReporte
{
    public int PrestamoId { get; set; }
    public string Socio { get; set; } = string.Empty;
    public string DNI { get; set; } = string.Empty;
    public string Libro { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime FechaPrestamo { get; set; }

    [Display(Name = "Fecha límite")]
    [DataType(DataType.Date)]
    public DateTime FechaLimite { get; set; }

    [DataType(DataType.Date)]
    public DateTime? FechaDevolucion { get; set; }

    public string Estado { get; set; } = string.Empty;
}
