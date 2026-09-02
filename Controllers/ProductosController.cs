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

    public ProductosController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Producto>> ObtenerTodos(
        [FromQuery] string? filtroNombre = null,
        [FromQuery] string? ordenarPor = null)
    {
        var productos = _productoService.ObtenerTodos(filtroNombre, ordenarPor);
        return Ok(productos);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Producto> ObtenerPorId([FromRoute] int id)
    {
        var producto = _productoService.ObtenerPorId(id);

        if (producto is null)
        {
            return NotFound(new { mensaje = $"No existe un producto con Id {id}." });
        }

        return Ok(producto);
    }

    [HttpPost]
    [ApiKeyAuthorizationFilter("user", "admin")]
    public ActionResult<Producto> Crear([FromBody] CrearProductoDto productoDto)
    {
        var producto = _productoService.Crear(productoDto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = producto.Id }, producto);
    }

    [HttpPut("{id:int}")]
    [ApiKeyAuthorizationFilter("admin")]
    public IActionResult Actualizar([FromRoute] int id, [FromBody] ActualizarProductoDto productoDto)
    {
        var producto = _productoService.Actualizar(id, productoDto);

        if (producto is null)
        {
            return NotFound(new { mensaje = $"No existe un producto con Id {id}." });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ApiKeyAuthorizationFilter("admin")]
    public IActionResult Eliminar([FromRoute] int id)
    {
        var producto = _productoService.Eliminar(id);

        if (producto is null)
        {
            return NotFound(new { mensaje = $"No existe un producto con Id {id}." });
        }

        return NoContent();
    }
}
