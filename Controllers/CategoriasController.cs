using ApiProductos.Data;
using ApiProductos.DTOs;
using ApiProductos.Filters;
using ApiProductos.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiProductos.Controllers;

/// <summary>
/// Controlador para gestionar categorías de productos.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Constructor del controlador de categorías.
    /// </summary>
    /// <param name="context">Contexto de base de datos.</param>
    public CategoriasController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene todas las categorías registradas.
    /// </summary>
    /// <returns>Lista de categorías.</returns>
    /// <response code="200">Lista de categorías obtenida correctamente.</response>
    /// <response code="401">No se envió la API key requerida.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Categoria>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<IEnumerable<Categoria>> ObtenerTodas()
    {
        return Ok(_context.Categorias.ToList());
    }

    /// <summary>
    /// Obtiene una categoría por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la categoría.</param>
    /// <returns>La categoría solicitada.</returns>
    /// <response code="200">Categoría encontrada.</response>
    /// <response code="404">No existe una categoría con el id proporcionado.</response>
    /// <response code="401">No se envió la API key requerida.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Categoria), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<Categoria> ObtenerPorId([FromRoute] int id)
    {
        var categoria = _context.Categorias.FirstOrDefault(c => c.Id == id);
        if (categoria is null)
        {
            return NotFound(new { mensaje = $"No existe una categoria con Id {id}." });
        }
        return Ok(categoria);
    }

    /// <summary>
    /// Crea una nueva categoría.
    /// </summary>
    /// <param name="dto">Datos de la categoría a crear.</param>
    /// <returns>La categoría creada con su identificador asignado.</returns>
    /// <response code="201">Categoría creada correctamente.</response>
    /// <response code="400">Los datos enviados no cumplen con las validaciones.</response>
    /// <response code="403">API key insuficiente. Se requiere rol admin.</response>
    [HttpPost]
    [ApiKeyAuthorizationFilter("admin")]
    [ProducesResponseType(typeof(Categoria), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public ActionResult<Categoria> Crear([FromBody] CrearCategoriaDto dto)
    {
        var categoria = new Categoria { Nombre = dto.Nombre };
        _context.Categorias.Add(categoria);
        _context.SaveChanges();
        return CreatedAtAction(nameof(ObtenerPorId), new { id = categoria.Id }, categoria);
    }

    /// <summary>
    /// Actualiza una categoría existente.
    /// </summary>
    /// <param name="id">Identificador de la categoría a actualizar.</param>
    /// <param name="dto">Nuevos datos de la categoría.</param>
    /// <returns>Sin contenido si la actualización fue exitosa.</returns>
    /// <response code="204">Categoría actualizada correctamente.</response>
    /// <response code="400">Los datos enviados no cumplen con las validaciones.</response>
    /// <response code="404">No existe una categoría con el id proporcionado.</response>
    /// <response code="403">API key insuficiente. Se requiere rol admin.</response>
    [HttpPut("{id:int}")]
    [ApiKeyAuthorizationFilter("admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult Actualizar([FromRoute] int id, [FromBody] CrearCategoriaDto dto)
    {
        var categoria = _context.Categorias.FirstOrDefault(c => c.Id == id);
        if (categoria is null)
        {
            return NotFound(new { mensaje = $"No existe una categoria con Id {id}." });
        }
        categoria.Nombre = dto.Nombre;
        _context.SaveChanges();
        return NoContent();
    }

    /// <summary>
    /// Elimina una categoría por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la categoría a eliminar.</param>
    /// <returns>Sin contenido si la eliminación fue exitosa.</returns>
    /// <response code="204">Categoría eliminada correctamente.</response>
    /// <response code="404">No existe una categoría con el id proporcionado.</response>
    /// <response code="403">API key insuficiente. Se requiere rol admin.</response>
    [HttpDelete("{id:int}")]
    [ApiKeyAuthorizationFilter("admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult Eliminar([FromRoute] int id)
    {
        var categoria = _context.Categorias.FirstOrDefault(c => c.Id == id);
        if (categoria is null)
        {
            return NotFound(new { mensaje = $"No existe una categoria con Id {id}." });
        }
        _context.Categorias.Remove(categoria);
        _context.SaveChanges();
        return NoContent();
    }
}
