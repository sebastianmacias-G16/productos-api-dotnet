using ApiProductos.Data;
using ApiProductos.DTOs;
using ApiProductos.Filters;
using ApiProductos.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiProductos.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Categoria>> ObtenerTodas()
    {
        return Ok(_context.Categorias.ToList());
    }

    [HttpGet("{id:int}")]
    public ActionResult<Categoria> ObtenerPorId([FromRoute] int id)
    {
        var categoria = _context.Categorias.FirstOrDefault(c => c.Id == id);
        if (categoria is null)
        {
            return NotFound(new { mensaje = $"No existe una categoria con Id {id}." });
        }
        return Ok(categoria);
    }

    [HttpPost]
    [ApiKeyAuthorizationFilter("admin")]
    public ActionResult<Categoria> Crear([FromBody] CrearCategoriaDto dto)
    {
        var categoria = new Categoria { Nombre = dto.Nombre };
        _context.Categorias.Add(categoria);
        _context.SaveChanges();
        return CreatedAtAction(nameof(ObtenerPorId), new { id = categoria.Id }, categoria);
    }

    [HttpPut("{id:int}")]
    [ApiKeyAuthorizationFilter("admin")]
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

    [HttpDelete("{id:int}")]
    [ApiKeyAuthorizationFilter("admin")]
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
