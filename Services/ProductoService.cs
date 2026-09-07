using ApiProductos.DTOs;
using ApiProductos.Models;
using Microsoft.Extensions.Logging;

namespace ApiProductos.Services;

public class ProductoService : IProductoService
{
    private static readonly List<Producto> Productos = new();
    private static int siguienteId = 1;
    private readonly ILogger<ProductoService> _logger;

    public ProductoService(ILogger<ProductoService> logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<Producto>> ObtenerTodosAsync(string? filtroNombre = null, string? ordenarPor = null)
    {
        _logger.LogInformation("Obteniendo todos los productos. Filtro: {FiltroNombre}, OrdenarPor: {OrdenarPor}", filtroNombre, ordenarPor);

        await Task.Delay(10);

        var query = Productos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtroNombre))
        {
            query = query.Where(p => p.Nombre.Contains(filtroNombre, StringComparison.OrdinalIgnoreCase));
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

        return query.ToList();
    }

    public async Task<Producto?> ObtenerPorIdAsync(int id)
    {
        _logger.LogInformation("Buscando producto con Id {Id}", id);

        await Task.Delay(10);

        return Productos.FirstOrDefault(p => p.Id == id);
    }

    public async Task<Producto> CrearAsync(CrearProductoDto productoDto)
    {
        _logger.LogInformation("Creando producto: {Nombre}", productoDto.Nombre);

        await Task.Delay(10);

        var producto = new Producto
        {
            Id = siguienteId++,
            Nombre = productoDto.Nombre,
            Precio = productoDto.Precio,
            Stock = productoDto.Stock
        };

        Productos.Add(producto);
        _logger.LogInformation("Producto creado con Id {Id}", producto.Id);

        return producto;
    }

    public async Task<Producto?> ActualizarAsync(int id, ActualizarProductoDto productoDto)
    {
        _logger.LogInformation("Actualizando producto con Id {Id}", id);

        await Task.Delay(10);

        var producto = await ObtenerPorIdAsync(id);
        if (producto is null)
        {
            _logger.LogWarning("Producto con Id {Id} no encontrado para actualizar", id);
            return null;
        }

        producto.Nombre = productoDto.Nombre;
        producto.Precio = productoDto.Precio;
        producto.Stock = productoDto.Stock;

        _logger.LogInformation("Producto con Id {Id} actualizado correctamente", id);

        return producto;
    }

    public async Task<Producto?> EliminarAsync(int id)
    {
        _logger.LogInformation("Eliminando producto con Id {Id}", id);

        await Task.Delay(10);

        var producto = await ObtenerPorIdAsync(id);
        if (producto is null)
        {
            _logger.LogWarning("Producto con Id {Id} no encontrado para eliminar", id);
            return null;
        }

        Productos.Remove(producto);
        _logger.LogInformation("Producto con Id {Id} eliminado correctamente", id);

        return producto;
    }
}
