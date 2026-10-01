using ApiProductos.Consultas;
using ApiProductos.Data;
using ApiProductos.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiProductos.Controllers;

/// <summary>
/// Controlador para ejecutar consultas SQL directas sobre la base de datos.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class ConsultasController : ControllerBase
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Constructor del controlador de consultas.
    /// </summary>
    /// <param name="context">Contexto de base de datos.</param>
    public ConsultasController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene todas las categorías registradas mediante consulta SQL directa.
    /// </summary>
    /// <returns>Lista de categorías.</returns>
    /// <response code="200">Lista de categorías obtenida correctamente.</response>
    /// <response code="401">No se envió la API key requerida.</response>
    [HttpGet("categorias")]
    [ApiKeyAuthorizationFilter("user", "admin")]
    [ProducesResponseType(typeof(IEnumerable<CategoriaQueryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

    /// <summary>
    /// Obtiene todos los productos registrados mediante consulta SQL directa.
    /// </summary>
    /// <returns>Lista de productos.</returns>
    /// <response code="200">Lista de productos obtenida correctamente.</response>
    /// <response code="401">No se envió la API key requerida.</response>
    [HttpGet("productos")]
    [ApiKeyAuthorizationFilter("user", "admin")]
    [ProducesResponseType(typeof(IEnumerable<ProductoQueryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

    /// <summary>
    /// Obtiene todos los productos con su categoría asociada mediante INNER JOIN.
    /// </summary>
    /// <returns>Lista de productos con información de su categoría.</returns>
    /// <response code="200">Lista de productos con categoría obtenida correctamente.</response>
    /// <response code="401">No se envió la API key requerida.</response>
    [HttpGet("productos/con-categorias")]
    [ApiKeyAuthorizationFilter("user", "admin")]
    [ProducesResponseType(typeof(IEnumerable<ProductoConCategoriaQueryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
