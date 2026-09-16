-- ============================================
-- CONSULTAS SQL MANUALES SOBRE LA BASE DE DATOS
-- Proyecto: ApiProductos
-- Motor: SQLite
-- ============================================

-- ============================================
-- 1. LISTAR TODOS LOS PRODUCTOS
-- ============================================
SELECT Id, Nombre, Precio, Stock, CategoriaId
FROM Productos;


-- ============================================
-- 2. LISTAR PRODUCTOS CON NOMBRE DE CATEGORIA (JOIN)
-- ============================================
-- JOIN simple entre Productos y Categorias
-- Muestra el nombre del producto, su precio, stock y el nombre de su categoria
SELECT 
    p.Id AS ProductoId,
    p.Nombre AS ProductoNombre,
    p.Precio,
    p.Stock,
    c.Id AS CategoriaId,
    c.Nombre AS CategoriaNombre
FROM Productos p
INNER JOIN Categorias c ON p.CategoriaId = c.Id;


-- ============================================
-- 3. LISTAR PRODUCTOS CON FILTRO POR NOMBRE
-- ============================================
-- Filtra productos que contengan "teclado" en el nombre (case insensitive en SQLite)
SELECT Id, Nombre, Precio, Stock, CategoriaId
FROM Productos
WHERE Nombre LIKE '%teclado%';


-- ============================================
-- 4. LISTAR PRODUCTOS ORDENADOS POR PRECIO DESCENDENTE
-- ============================================
SELECT Id, Nombre, Precio, Stock, CategoriaId
FROM Productos
ORDER BY Precio DESC;


-- ============================================
-- 5. ACTUALIZAR STOCK DE UN PRODUCTO
-- ============================================
UPDATE Productos
SET Stock = 20
WHERE Id = 1;


-- ============================================
-- 6. ELIMINAR UN PRODUCTO POR ID
-- ============================================
DELETE FROM Productos
WHERE Id = 1;


-- ============================================
-- 7. CONTAR CUANTOS PRODUCTOS HAY POR CATEGORIA
-- ============================================
-- Agrupa productos por categoria y cuenta cuantos hay en cada una
SELECT 
    c.Nombre AS Categoria,
    COUNT(p.Id) AS CantidadProductos
FROM Categorias c
LEFT JOIN Productos p ON c.Id = p.CategoriaId
GROUP BY c.Id, c.Nombre;


-- ============================================
-- 8. PRODUCTOS SIN CATEGORIA ASIGNADA
-- ============================================
-- Lista productos que no tienen categoria (CategoriaId es NULL)
SELECT Id, Nombre, Precio, Stock
FROM Productos
WHERE CategoriaId IS NULL;


-- ============================================
-- 9. PROCEDIMIENTO ALMACENADO (SQL Server)
-- ============================================
-- En SQL Server se puede crear un procedimiento almacenado para filtrar productos
-- por categoria. A continuacion se muestra la sintaxis T-SQL:

-- CREATE PROCEDURE ObtenerProductosPorCategoria
--     @CategoriaId INT
-- AS
-- BEGIN
--     SELECT 
--         p.Id,
--         p.Nombre,
--         p.Precio,
--         p.Stock,
--         c.Nombre AS CategoriaNombre
--     FROM Productos p
--     INNER JOIN Categorias c ON p.CategoriaId = c.Id
--     WHERE p.CategoriaId = @CategoriaId;
-- END
--
-- GO

-- Para ejecutarlo:
-- EXEC ObtenerProductosPorCategoria @CategoriaId = 1;

-- ============================================
-- 10. PROCEDIMIENTO ALMACENADO ALTERNATIVO (crear producto)
-- ============================================
-- CREATE PROCEDURE CrearProducto
--     @Nombre NVARCHAR(100),
--     @Precio DECIMAL(18,2),
--     @Stock INT,
--     @CategoriaId INT = NULL
-- AS
-- BEGIN
--     INSERT INTO Productos (Nombre, Precio, Stock, CategoriaId)
--     VALUES (@Nombre, @Precio, @Stock, @CategoriaId);
--
--     SELECT * FROM Productos WHERE Id = last_insert_rowid();
-- END
--
-- GO

-- ============================================
-- NOTA SOBRE SQLITE
-- ============================================
-- SQLite no soporta procedimientos almacenados nativamente.
-- Las consultas anteriores se ejecutan directamente desde la aplicacion
-- usando ADO.NET, Dapper o EF Core.
-- Si se migra a SQL Server, se pueden crear procedimientos almacenados
-- como se muestra en los ejemplos 9 y 10.
