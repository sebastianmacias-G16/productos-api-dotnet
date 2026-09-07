using ApiProductos.DTOs;
using ApiProductos.Filters;
using ApiProductos.Models;
using ApiProductos.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiProductos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _productoService;
    private readonly ILogger<ProductosController> _logger;

    public ProductosController(IProductoService productoService, ILogger<ProductosController> logger)
    {
        _productoService = productoService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Producto>>> ObtenerTodos(
        [FromQuery] string? filtroNombre = null,
        [FromQuery] string? ordenarPor = null)
    {
        _logger.LogInformation("GET /api/productos - Obteniendo todos los productos");
        var productos = await _productoService.ObtenerTodosAsync(filtroNombre, ordenarPor);
        return Ok(productos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Producto>> ObtenerPorId([FromRoute] int id)
    {
        _logger.LogInformation("GET /api/productos/{Id} - Buscando producto", id);
        var producto = await _productoService.ObtenerPorIdAsync(id);

        if (producto is null)
        {
            _logger.LogWarning("Producto con Id {Id} no encontrado", id);
            return NotFound(new { mensaje = $"No existe un producto con Id {id}." });
        }

        return Ok(producto);
    }

    [HttpPost]
    [ApiKeyAuthorizationFilter("user", "admin")]
    public async Task<ActionResult<Producto>> Crear([FromBody] CrearProductoDto productoDto)
    {
        _logger.LogInformation("POST /api/productos - Creando producto: {Nombre}", productoDto.Nombre);
        var producto = await _productoService.CrearAsync(productoDto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = producto.Id }, producto);
    }

    [HttpPut("{id:int}")]
    [ApiKeyAuthorizationFilter("admin")]
    public async Task<IActionResult> Actualizar([FromRoute] int id, [FromBody] ActualizarProductoDto productoDto)
    {
        _logger.LogInformation("PUT /api/productos/{Id} - Actualizando producto", id);
        var producto = await _productoService.ActualizarAsync(id, productoDto);

        if (producto is null)
        {
            _logger.LogWarning("Producto con Id {Id} no encontrado para actualizar", id);
            return NotFound(new { mensaje = $"No existe un producto con Id {id}." });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ApiKeyAuthorizationFilter("admin")]
    public async Task<IActionResult> Eliminar([FromRoute] int id)
    {
        _logger.LogInformation("DELETE /api/productos/{Id} - Eliminando producto", id);
        var producto = await _productoService.EliminarAsync(id);

        if (producto is null)
        {
            _logger.LogWarning("Producto con Id {Id} no encontrado para eliminar", id);
            return NotFound(new { mensaje = $"No existe un producto con Id {id}." });
        }

        return NoContent();
    }
}
