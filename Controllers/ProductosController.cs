using ApiProductos.DTOs;
using ApiProductos.Filters;
using ApiProductos.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiProductos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private static readonly List<Producto> Productos = new();
    private static int siguienteId = 1;

    [HttpGet]
    public ActionResult<IEnumerable<Producto>> ObtenerTodos()
    {
        return Ok(Productos);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Producto> ObtenerPorId([FromRoute] int id)
    {
        var producto = Productos.FirstOrDefault(producto => producto.Id == id);

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
        var producto = new Producto
        {
            Id = siguienteId++,
            Nombre = productoDto.Nombre,
            Precio = productoDto.Precio,
            Stock = productoDto.Stock
        };

        Productos.Add(producto);

        return CreatedAtAction(nameof(ObtenerPorId), new { id = producto.Id }, producto);
    }

    [HttpPut("{id:int}")]
    [ApiKeyAuthorizationFilter("admin")]
    public IActionResult Actualizar([FromRoute] int id, [FromBody] ActualizarProductoDto productoDto)
    {
        var producto = Productos.FirstOrDefault(producto => producto.Id == id);

        if (producto is null)
        {
            return NotFound(new { mensaje = $"No existe un producto con Id {id}." });
        }

        producto.Nombre = productoDto.Nombre;
        producto.Precio = productoDto.Precio;
        producto.Stock = productoDto.Stock;

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ApiKeyAuthorizationFilter("admin")]
    public IActionResult Eliminar([FromRoute] int id)
    {
        var producto = Productos.FirstOrDefault(producto => producto.Id == id);

        if (producto is null)
        {
            return NotFound(new { mensaje = $"No existe un producto con Id {id}." });
        }

        Productos.Remove(producto);
        return NoContent();
    }
}
