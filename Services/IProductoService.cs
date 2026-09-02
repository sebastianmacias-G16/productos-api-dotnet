using ApiProductos.DTOs;
using ApiProductos.Models;

namespace ApiProductos.Services;

public interface IProductoService
{
    IEnumerable<Producto> ObtenerTodos(string? filtroNombre = null, string? ordenarPor = null);
    Producto? ObtenerPorId(int id);
    Producto Crear(CrearProductoDto productoDto);
    Producto? Actualizar(int id, ActualizarProductoDto productoDto);
    Producto? Eliminar(int id);
}
