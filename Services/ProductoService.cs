using ApiProductos.Data;
using ApiProductos.DTOs;
using ApiProductos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiProductos.Services;

public class ProductoService : IProductoService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProductoService> _logger;

    public ProductoService(AppDbContext context, ILogger<ProductoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Producto>> ObtenerTodosAsync(string? filtroNombre = null, string? ordenarPor = null)
    {
        _logger.LogInformation("Obteniendo todos los productos. Filtro: {FiltroNombre}, OrdenarPor: {OrdenarPor}", filtroNombre, ordenarPor);

        var query = _context.Productos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtroNombre))
        {
            query = query.Where(p => p.Nombre.Contains(filtroNombre));
        }

        query = ordenarPor?.ToLower() switch
        {
            "precio" => query.OrderBy(p => p.Precio),
            "-precio" => query.OrderByDescending(p => p.Precio),
            "stock" => query.OrderBy(p => p.Stock),
            "-stock" => query.OrderByDescending(p => p.Stock),
            "nombre" => query.OrderBy(p => p.Nombre),
            "-nombre" => query.OrderByDescending(p => p.Nombre),
            _ => query
        };

        return await query.ToListAsync();
    }

    public async Task<Producto?> ObtenerPorIdAsync(int id)
    {
        _logger.LogInformation("Buscando producto con Id {Id}", id);

        return await _context.Productos.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Producto> CrearAsync(CrearProductoDto productoDto)
    {
        _logger.LogInformation("Creando producto: {Nombre}", productoDto.Nombre);

        var producto = new Producto
        {
            Nombre = productoDto.Nombre,
            Precio = productoDto.Precio,
            Stock = productoDto.Stock
        };

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Producto creado con Id {Id}", producto.Id);

        return producto;
    }

    public async Task<Producto?> ActualizarAsync(int id, ActualizarProductoDto productoDto)
    {
        _logger.LogInformation("Actualizando producto con Id {Id}", id);

        var producto = await ObtenerPorIdAsync(id);
        if (producto is null)
        {
            _logger.LogWarning("Producto con Id {Id} no encontrado para actualizar", id);
            return null;
        }

        producto.Nombre = productoDto.Nombre;
        producto.Precio = productoDto.Precio;
        producto.Stock = productoDto.Stock;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Producto con Id {Id} actualizado correctamente", id);

        return producto;
    }

    public async Task<Producto?> EliminarAsync(int id)
    {
        _logger.LogInformation("Eliminando producto con Id {Id}", id);

        var producto = await ObtenerPorIdAsync(id);
        if (producto is null)
        {
            _logger.LogWarning("Producto con Id {Id} no encontrado para eliminar", id);
            return null;
        }

        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Producto con Id {Id} eliminado correctamente", id);

        return producto;
    }
}
