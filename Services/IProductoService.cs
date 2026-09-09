using ApiProductos.DTOs;
using ApiProductos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiProductos.Services;

public interface IProductoService
{
    Task<IEnumerable<Producto>> ObtenerTodosAsync(string? filtroNombre = null, string? ordenarPor = null);
    Task<Producto?> ObtenerPorIdAsync(int id);
    Task<Producto> CrearAsync(CrearProductoDto productoDto);
    Task<Producto?> ActualizarAsync(int id, ActualizarProductoDto productoDto);
    Task<Producto?> EliminarAsync(int id);
}
