using ApiProductos.DTOs;
using ApiProductos.Models;

namespace ApiProductos.Services;

public class ProductoService : IProductoService
{
    private static readonly List<Producto> Productos = new();
    private static int siguienteId = 1;

    public IEnumerable<Producto> ObtenerTodos(string? filtroNombre = null, string? ordenarPor = null)
    {
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

    public Producto? ObtenerPorId(int id)
    {
        return Productos.FirstOrDefault(p => p.Id == id);
    }

    public Producto Crear(CrearProductoDto productoDto)
    {
        var producto = new Producto
        {
            Id = siguienteId++,
            Nombre = productoDto.Nombre,
            Precio = productoDto.Precio,
            Stock = productoDto.Stock
        };

        Productos.Add(producto);
        return producto;
    }

    public Producto? Actualizar(int id, ActualizarProductoDto productoDto)
    {
        var producto = ObtenerPorId(id);
        if (producto is null)
        {
            return null;
        }

        producto.Nombre = productoDto.Nombre;
        producto.Precio = productoDto.Precio;
        producto.Stock = productoDto.Stock;

        return producto;
    }

    public Producto? Eliminar(int id)
    {
        var producto = ObtenerPorId(id);
        if (producto is null)
        {
            return null;
        }

        Productos.Remove(producto);
        return producto;
    }
}
