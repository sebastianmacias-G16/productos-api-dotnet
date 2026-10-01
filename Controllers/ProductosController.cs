using ApiProductos.DTOs;
using ApiProductos.Filters;
using ApiProductos.Models;
using ApiProductos.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiProductos.Controllers;

/// <summary>
/// Controlador para gestionar productos.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _productoService;
    private readonly ILogger<ProductosController> _logger;

    /// <summary>
    /// Constructor del controlador de productos.
    /// </summary>
    /// <param name="productoService">Servicio de lógica de negocio de productos.</param>
    /// <param name="logger">Logger para registrar eventos.</param>
    public ProductosController(IProductoService productoService, ILogger<ProductosController> logger)
    {
        _productoService = productoService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los productos, con filtrado y ordenamiento opcionales.
    /// </summary>
    /// <param name="filtroNombre">Texto para filtrar productos por nombre.</param>
    /// <param name="ordenarPor">Criterio de ordenamiento: precio, -precio, stock, -stock, nombre, -nombre.</param>
    /// <returns>Lista de productos.</returns>
    /// <response code="200">Lista de productos obtenida correctamente.</response>
    /// <response code="401">No se envió la API key requerida.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Producto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<Producto>>> ObtenerTodos(
        [FromQuery] string? filtroNombre = null,
        [FromQuery] string? ordenarPor = null)
    {
        _logger.LogInformation("GET /api/v1/productos - Obteniendo todos los productos");
        var productos = await _productoService.ObtenerTodosAsync(filtroNombre, ordenarPor);
        return Ok(productos);
    }

    /// <summary>
    /// Obtiene un producto por su identificador.
    /// </summary>
    /// <param name="id">Identificador del producto.</param>
    /// <returns>El producto solicitado.</returns>
    /// <response code="200">Producto encontrado.</response>
    /// <response code="404">No existe un producto con el id proporcionado.</response>
    /// <response code="401">No se envió la API key requerida.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Producto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Producto>> ObtenerPorId([FromRoute] int id)
    {
        _logger.LogInformation("GET /api/v1/productos/{Id} - Buscando producto", id);
        var producto = await _productoService.ObtenerPorIdAsync(id);

        if (producto is null)
        {
            _logger.LogWarning("Producto con Id {Id} no encontrado", id);
            return NotFound(new { mensaje = $"No existe un producto con Id {id}." });
        }

        return Ok(producto);
    }

    /// <summary>
    /// Crea un nuevo producto.
    /// </summary>
    /// <param name="productoDto">Datos del producto a crear.</param>
    /// <returns>El producto creado con su identificador asignado.</returns>
    /// <response code="201">Producto creado correctamente.</response>
    /// <response code="400">Los datos enviados no cumplen con las validaciones.</response>
    /// <response code="403">API key insuficiente. Se requiere rol user o admin.</response>
    [HttpPost]
    [ApiKeyAuthorizationFilter("user", "admin")]
    [ProducesResponseType(typeof(Producto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Producto>> Crear([FromBody] CrearProductoDto productoDto)
    {
        _logger.LogInformation("POST /api/v1/productos - Creando producto: {Nombre}", productoDto.Nombre);
        var producto = await _productoService.CrearAsync(productoDto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = producto.Id }, producto);
    }

    /// <summary>
    /// Actualiza un producto existente.
    /// </summary>
    /// <param name="id">Identificador del producto a actualizar.</param>
    /// <param name="productoDto">Nuevos datos del producto.</param>
    /// <returns>Sin contenido si la actualización fue exitosa.</returns>
    /// <response code="204">Producto actualizado correctamente.</response>
    /// <response code="400">Los datos enviados no cumplen con las validaciones.</response>
    /// <response code="404">No existe un producto con el id proporcionado.</response>
    /// <response code="403">API key insuficiente. Se requiere rol admin.</response>
    [HttpPut("{id:int}")]
    [ApiKeyAuthorizationFilter("admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Actualizar([FromRoute] int id, [FromBody] ActualizarProductoDto productoDto)
    {
        _logger.LogInformation("PUT /api/v1/productos/{Id} - Actualizando producto", id);
        var producto = await _productoService.ActualizarAsync(id, productoDto);

        if (producto is null)
        {
            _logger.LogWarning("Producto con Id {Id} no encontrado para actualizar", id);
            return NotFound(new { mensaje = $"No existe un producto con Id {id}." });
        }

        return NoContent();
    }

    /// <summary>
    /// Elimina un producto por su identificador.
    /// </summary>
    /// <param name="id">Identificador del producto a eliminar.</param>
    /// <returns>Sin contenido si la eliminación fue exitosa.</returns>
    /// <response code="204">Producto eliminado correctamente.</response>
    /// <response code="404">No existe un producto con el id proporcionado.</response>
    /// <response code="403">API key insuficiente. Se requiere rol admin.</response>
    [HttpDelete("{id:int}")]
    [ApiKeyAuthorizationFilter("admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Eliminar([FromRoute] int id)
    {
        _logger.LogInformation("DELETE /api/v1/productos/{Id} - Eliminando producto", id);
        var producto = await _productoService.EliminarAsync(id);

        if (producto is null)
        {
            _logger.LogWarning("Producto con Id {Id} no encontrado para eliminar", id);
            return NotFound(new { mensaje = $"No existe un producto con Id {id}." });
        }

        return NoContent();
    }
}
