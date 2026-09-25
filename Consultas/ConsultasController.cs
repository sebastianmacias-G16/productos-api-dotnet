using ApiProductos.Consultas;
using ApiProductos.Data;
using ApiProductos.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiProductos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsultasController : ControllerBase
{
    private readonly AppDbContext _context;

    public ConsultasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("categorias")]
    [ApiKeyAuthorizationFilter("user", "admin")]
    public ActionResult<IEnumerable<CategoriaQueryDto>> ObtenerCategorias()
    {
        var sql = "SELECT Id, Nombre FROM Categorias";

        var categorias = _context.Categorias
            .FromSqlRaw(sql)
            .Select(c => new CategoriaQueryDto
            {
                Id = c.Id,
                Nombre = c.Nombre
            })
            .ToList();

        return Ok(categorias);
    }

    [HttpGet("productos")]
    [ApiKeyAuthorizationFilter("user", "admin")]
    public ActionResult<IEnumerable<ProductoQueryDto>> ObtenerProductos()
    {
        var sql = "SELECT Id, Nombre, Precio, Stock, CategoriaId FROM Productos";

        var productos = _context.Productos
            .FromSqlRaw(sql)
            .Select(p => new ProductoQueryDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Precio = p.Precio,
                Stock = p.Stock,
                CategoriaId = p.CategoriaId
            })
            .ToList();

        return Ok(productos);
    }

    [HttpGet("productos/con-categorias")]
    [ApiKeyAuthorizationFilter("user", "admin")]
    public async Task<ActionResult<IEnumerable<ProductoConCategoriaQueryDto>>> ObtenerProductosConCategoria()
    {
        var sql = @"
            SELECT
                p.Id AS ProductoId,
                p.Nombre AS Producto,
                p.Precio,
                p.Stock,
                c.Id AS CategoriaId,
                c.Nombre AS Categoria
            FROM Productos p
            INNER JOIN Categorias c ON p.CategoriaId = c.Id";

        var productos = await _context.Database
            .SqlQueryRaw<ProductoConCategoriaQueryDto>(sql)
            .ToListAsync();

        return Ok(productos);
    }
}
