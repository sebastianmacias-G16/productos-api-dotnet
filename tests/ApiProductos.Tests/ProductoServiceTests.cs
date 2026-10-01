using ApiProductos.Data;
using ApiProductos.DTOs;
using ApiProductos.Models;
using ApiProductos.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ApiProductos.Tests;

/// <summary>
/// Pruebas unitarias para ProductoService.
/// Se utilizan dos enfoques:
/// 1. DbContext en memoria para probar flujos completos sin base de datos real.
/// 2. Moq para simular el DbContext y probar un caso aislado.
/// </summary>
public class ProductoServiceTests
{
    #region Helpers con DbContext en memoria

    private AppDbContext CrearDbContextEnMemoria()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        return context;
    }

    private ProductoService CrearServicio(AppDbContext context)
    {
        var mockLogger = new Mock<ILogger<ProductoService>>();
        return new ProductoService(context, mockLogger.Object);
    }

    #endregion

    #region Pruebas con DbContext en memoria

    [Fact]
    public async Task ObtenerTodosAsync_SinFiltros_DeberiaRetornarTodosLosProductos()
    {
        var context = CrearDbContextEnMemoria();
        var servicio = CrearServicio(context);

        context.Productos.Add(new Producto { Nombre = "Teclado", Precio = 150000, Stock = 10 });
        context.Productos.Add(new Producto { Nombre = "Mouse", Precio = 50000, Stock = 25 });
        await context.SaveChangesAsync();

        var resultado = await servicio.ObtenerTodosAsync();

        Assert.Equal(2, resultado.Count());
    }

    [Fact]
    public async Task ObtenerTodosAsync_ConFiltroNombre_DeberiaFiltrarCorrectamente()
    {
        var context = CrearDbContextEnMemoria();
        var servicio = CrearServicio(context);

        context.Productos.Add(new Producto { Nombre = "TEclado Mecánico", Precio = 150000, Stock = 10 });
        context.Productos.Add(new Producto { Nombre = "Mouse Gamer", Precio = 50000, Stock = 25 });
        await context.SaveChangesAsync();

        var resultado = await servicio.ObtenerTodosAsync(filtroNombre: "TEclado");

        Assert.Single(resultado);
        Assert.Equal("TEclado Mecánico", resultado.First().Nombre);
    }

    [Fact]
    public async Task ObtenerTodosAsync_ConOrdenamiento_DeberiaOrdenarCorrectamente()
    {
        var context = CrearDbContextEnMemoria();
        var servicio = CrearServicio(context);

        context.Productos.Add(new Producto { Nombre = "A", Precio = 100, Stock = 10 });
        context.Productos.Add(new Producto { Nombre = "B", Precio = 50, Stock = 20 });
        await context.SaveChangesAsync();

        var resultado = (await servicio.ObtenerTodosAsync(ordenarPor: "-precio")).ToList();

        Assert.Equal("A", resultado[0].Nombre);
        Assert.Equal("B", resultado[1].Nombre);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_ConIdExistente_DeberiaRetornarProducto()
    {
        var context = CrearDbContextEnMemoria();
        var servicio = CrearServicio(context);

        var producto = new Producto { Nombre = "Teclado", Precio = 150000, Stock = 10 };
        context.Productos.Add(producto);
        await context.SaveChangesAsync();

        var resultado = await servicio.ObtenerPorIdAsync(producto.Id);

        Assert.NotNull(resultado);
        Assert.Equal("Teclado", resultado!.Nombre);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_ConIdInexistente_DeberiaRetornarNull()
    {
        var context = CrearDbContextEnMemoria();
        var servicio = CrearServicio(context);

        var resultado = await servicio.ObtenerPorIdAsync(999);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task CrearAsync_DeberiaCrearProductoYAsignarId()
    {
        var context = CrearDbContextEnMemoria();
        var servicio = CrearServicio(context);

        var dto = new CrearProductoDto
        {
            Nombre = "Teclado",
            Precio = 150000,
            Stock = 10,
            CategoriaId = null
        };

        var producto = await servicio.CrearAsync(dto);

        Assert.NotEqual(0, producto.Id);
        Assert.Equal("Teclado", producto.Nombre);
        Assert.Equal(150000, producto.Precio);
        Assert.Equal(10, producto.Stock);
    }

    [Fact]
    public async Task ActualizarAsync_ConIdExistente_DeberiaActualizarProducto()
    {
        var context = CrearDbContextEnMemoria();
        var servicio = CrearServicio(context);

        var producto = new Producto { Nombre = "Teclado", Precio = 150000, Stock = 10 };
        context.Productos.Add(producto);
        await context.SaveChangesAsync();

        var dto = new ActualizarProductoDto
        {
            Nombre = "Teclado RGB",
            Precio = 175000,
            Stock = 8,
            CategoriaId = null
        };

        var resultado = await servicio.ActualizarAsync(producto.Id, dto);

        Assert.NotNull(resultado);
        Assert.Equal("Teclado RGB", resultado!.Nombre);
        Assert.Equal(175000, resultado.Precio);
    }

    [Fact]
    public async Task ActualizarAsync_ConIdInexistente_DeberiaRetornarNull()
    {
        var context = CrearDbContextEnMemoria();
        var servicio = CrearServicio(context);

        var dto = new ActualizarProductoDto
        {
            Nombre = "Teclado RGB",
            Precio = 175000,
            Stock = 8,
            CategoriaId = null
        };

        var resultado = await servicio.ActualizarAsync(999, dto);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task EliminarAsync_ConIdExistente_DeberiaEliminarProducto()
    {
        var context = CrearDbContextEnMemoria();
        var servicio = CrearServicio(context);

        var producto = new Producto { Nombre = "Teclado", Precio = 150000, Stock = 10 };
        context.Productos.Add(producto);
        await context.SaveChangesAsync();

        var resultado = await servicio.EliminarAsync(producto.Id);

        Assert.NotNull(resultado);
        Assert.Equal("Teclado", resultado!.Nombre);

        var productoEliminado = await context.Productos.FindAsync(producto.Id);
        Assert.Null(productoEliminado);
    }

    [Fact]
    public async Task EliminarAsync_ConIdInexistente_DeberiaRetornarNull()
    {
        var context = CrearDbContextEnMemoria();
        var servicio = CrearServicio(context);

        var resultado = await servicio.EliminarAsync(999);

        Assert.Null(resultado);
    }

    #endregion

    #region Prueba con Moq

    // Nota: Moq no puede mockear directamente DbContext porque sus propiedades
    // como DbSet<Producto> no son virtuales. Para usar Moq con EF Core,
    // lo recomendado es crear una interfaz (ej. IProductoRepository) y que
    // el servicio dependa de esa interfaz en lugar de AppDbContext directamente.
    // Por simplicidad en este proyecto, las pruebas usan InMemory database.

    #endregion
}
