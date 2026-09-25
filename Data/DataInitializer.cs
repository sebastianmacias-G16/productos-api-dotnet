using ApiProductos.Data;
using ApiProductos.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiProductos.Data;

public static class DataInitializer
{
    public static void Inicializar(AppDbContext context)
    {
        context.Database.EnsureCreated();

        if (!context.Categorias.Any())
        {
            var perifericos = new Categoria { Nombre = "Perifericos" };
            var pantallas = new Categoria { Nombre = "Pantallas" };
            var computadores = new Categoria { Nombre = "Computadores" };
            var accesorios = new Categoria { Nombre = "Accesorios" };

            context.Categorias.AddRange(perifericos, pantallas, computadores, accesorios);
            context.SaveChanges();

            context.Productos.AddRange(
                new Producto { Nombre = "Teclado Mecanico", Precio = 150000, Stock = 10, CategoriaId = perifericos.Id },
                new Producto { Nombre = "Mouse Gamer", Precio = 50000, Stock = 25, CategoriaId = perifericos.Id },
                new Producto { Nombre = "Monitor 24", Precio = 500000, Stock = 5, CategoriaId = pantallas.Id },
                new Producto { Nombre = "Monitor Gamer 27", Precio = 1200000, Stock = 7, CategoriaId = pantallas.Id },
                new Producto { Nombre = "Laptop", Precio = 2500000, Stock = 4, CategoriaId = computadores.Id },
                new Producto { Nombre = "PC Gamer", Precio = 4500000, Stock = 3, CategoriaId = computadores.Id },
                new Producto { Nombre = "Cable HDMI", Precio = 30000, Stock = 20, CategoriaId = accesorios.Id }
            );

            context.SaveChanges();
        }
    }
}
