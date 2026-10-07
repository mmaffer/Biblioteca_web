using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Web.Models;

public class Libro
{
    public int LibroId { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "El título debe tener entre 2 y 150 caracteres.")]
    [DataType(DataType.Text)]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El ISBN es obligatorio.")]
    [StringLength(20, MinimumLength = 5, ErrorMessage = "El ISBN debe tener entre 5 y 20 caracteres.")]
    [DataType(DataType.Text)]
    public string ISBN { get; set; } = string.Empty;

    [Display(Name = "Autor")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un autor.")]
    public int AutorId { get; set; }

    // Solo lectura: viene del INNER JOIN con Autores.
    [Display(Name = "Autor")]
    public string? AutorNombre { get; set; }

    [Range(0, 1000, ErrorMessage = "Los ejemplares deben estar entre 0 y 1000.")]
    public int Ejemplares { get; set; }

    public bool Activo { get; set; } = true;
}
