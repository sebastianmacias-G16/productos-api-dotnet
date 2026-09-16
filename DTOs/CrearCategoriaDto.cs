using System.ComponentModel.DataAnnotations;

namespace ApiProductos.DTOs;

public class CrearCategoriaDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
    public string Nombre { get; set; } = string.Empty;
}
