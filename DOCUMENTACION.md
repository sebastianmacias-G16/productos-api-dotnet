# ApiProductos — Documentación del proyecto

## 1. Propósito

**ApiProductos** es una Web API REST desarrollada con ASP.NET Core (.NET 10) para administrar productos. Incluye operaciones CRUD, versionamiento simple de API, documentación Swagger/OpenAPI, manejo global de errores, logs, validaciones con DataAnnotations, autenticación por API key, filtrado y ordenamiento con LINQ, y persistencia real en SQLite mediante Entity Framework Core.

El proyecto sirve como práctica de:

- Desarrollo de una API REST con ASP.NET Core.
- Uso de controladores, rutas y métodos HTTP.
- Separación entre entidades y DTOs.
- Validación automática de solicitudes con DataAnnotations.
- Inyección de dependencias e interfaces.
- Entity Framework Core + SQLite + migraciones.
- LINQ para filtrado y ordenamiento.
- Manejo global de errores con middleware.
- Logs con `ILogger`.
- Autenticación y autorización básica con filtros personalizados.
- Documentación interactiva con Swagger / OpenAPI.
- Control de versiones con Git.

## 2. Tecnologías

| Tecnología | Uso en el proyecto |
|---|---|
| .NET 10 | Plataforma de ejecución y desarrollo. |
| ASP.NET Core | Framework para crear la API HTTP. |
| C# | Lenguaje de programación. |
| Entity Framework Core | ORM para persistencia en SQLite. |
| SQLite | Base de datos local. |
| Swashbuckle / OpenAPI | Documentación interactiva de la API. |
| Git | Control de versiones local y remoto. |

## 3. Estructura del proyecto

```text
ApiProductos/
├── Controllers/
│   ├── ProductosController.cs
│   └── CategoriasController.cs
├── DTOs/
│   ├── CrearProductoDto.cs
│   ├── ActualizarProductoDto.cs
│   └── CrearCategoriaDto.cs
├── Models/
│   ├── Producto.cs
│   └── Categoria.cs
├── Services/
│   ├── IProductoService.cs
│   └── ProductoService.cs
├── Data/
│   └── AppDbContext.cs
├── Filters/
│   └── ApiKeyAuthorizationFilter.cs
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs
├── Sql/
│   └── consultas.sql
├── Migrations/
│   ├── 20260909212953_InitialCreate.cs
│   ├── 20260909222214_initialcreate2.cs
│   ├── 20260916202857_AgregarCategoria.cs
│   └── AppDbContextModelSnapshot.cs
├── Properties/
│   └── launchSettings.json
├── ApiProductos.http
├── ApiProductos.csproj
├── appsettings.Development.json
├── appsettings.json
├── DOCUMENTACION.md
├── productos.db
└── Program.cs
```

## 4. Arquitectura y responsabilidades

### `Program.cs`

Punto de entrada. Registra servicios, configura el pipeline HTTP, Swagger y el middleware de excepciones:

```csharp
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=productos.db"));
builder.Services.AddScoped<IProductoService, ProductoService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();

app.Run();
```

### `Models/Producto.cs`

Entidad principal mapeada a la tabla `Productos`.

| Propiedad | Tipo | Descripción |
|---|---|---|
| `Id` | `int` | Identificador único autoincrementable. |
| `Nombre` | `string` | Nombre del producto. |
| `Precio` | `decimal` | Valor monetario del producto. |
| `Stock` | `int` | Unidades disponibles. |
| `CategoriaId` | `int?` | Foreign key nullable hacia `Categorias`. |
| `Categoria` | `Categoria?` | Navegación hacia la categoría. |

### `Models/Categoria.cs`

Entidad adicional que representa una categoría de producto.

| Propiedad | Tipo | Descripción |
|---|---|---|
| `Id` | `int` | Identificador único. |
| `Nombre` | `string` | Nombre de la categoría. |
| `Productos` | `List<Producto>` | Navegación inversa hacia los productos. |

### `DTOs/CrearProductoDto.cs`

Datos de entrada para crear un producto, con validaciones:

| Campo | Regla |
|---|---|
| `Nombre` | Obligatorio y con máximo 100 caracteres. |
| `Precio` | Debe ser mayor que cero. |
| `Stock` | No puede ser negativo. |
| `CategoriaId` | Opcional. |

### `DTOs/ActualizarProductoDto.cs`

Datos de entrada para actualizar un producto. mismas validaciones que `CrearProductoDto`.

### `DTOs/CrearCategoriaDto.cs`

Datos de entrada para crear una categoría.

| Campo | Regla |
|---|---|
| `Nombre` | Obligatorio y con máximo 50 caracteres. |

### `Services/IProductoService.cs`

Interfaz que define las operaciones del servicio de productos:

```csharp
Task<IEnumerable<Producto>> ObtenerTodosAsync(string? filtroNombre = null, string? ordenarPor = null);
Task<Producto?> ObtenerPorIdAsync(int id);
Task<Producto> CrearAsync(CrearProductoDto productoDto);
Task<Producto?> ActualizarAsync(int id, ActualizarProductoDto productoDto);
Task<Producto?> EliminarAsync(int id);
```

### `Services/ProductoService.cs`

Implementación de la lógica de negocio. Usa EF Core para consultar y persistir productos. Aplica LINQ para filtrar por nombre y ordenar por `precio`, `stock` o `nombre`. Registra logs con `ILogger<ProductoService>`.

### `Data/AppDbContext.cs`

DbContext configurado con SQLite. Expone `DbSet<Producto>` y `DbSet<Categoria>`. Configura la relación uno a muchos en `OnModelCreating`.

### `Filters/ApiKeyAuthorizationFilter.cs`

Filtro de autorización personalizado basado en header `X-API-KEY`:

| Header | Valor | Rol |
|---|---|---|
| `X-API-KEY` | `admin-key` | `admin` |
| `X-API-KEY` | `user-key` | `user` |

- Si falta el header → **401 Unauthorized**
- Si la key no coincide → **401 Unauthorized**
- Si la key no tiene el rol requerido → **403 Forbidden**

### `Middleware/ExceptionHandlingMiddleware.cs`

Middleware global que captura excepciones no controladas y devuelve una respuesta JSON estandarizada con status **500 Internal Server Error** e incluye `traceId` para tracing.

## 5. Versionamiento de la API

La API usa **versionamiento por URL**. Los controladores están Rutas `api/v1/[controller]`, por ejemplo:

- `GET /api/v1/productos`
- `GET /api/v1/categorias`

Esto permite tener múltiples versiones conviviendo en la misma aplicación. El prefijo `v1` se establece en la ruta de cada controlador.

## 6. Endpoints disponibles

### Productos (`/api/v1/productos`)

| Método | Ruta | Acción | Auth | Respuesta exitosa |
|---|---|---|---|---|
| `GET` | `/api/v1/productos` | Lista productos | API key | `200 OK` |
| `GET` | `/api/v1/productos/{id}` | Obtiene por id | API key | `200 OK` |
| `POST` | `/api/v1/productos` | Crea producto | `user` o `admin` | `201 Created` |
| `PUT` | `/api/v1/productos/{id}` | Actualiza producto | `admin` | `204 No Content` |
| `DELETE` | `/api/v1/productos/{id}` | Elimina producto | `admin` | `204 No Content` |

**Filtrado y ordenamiento:**

```http
GET /api/v1/productos?filtroNombre=teclado&ordenarPor=-precio
```

| Query param | Valores posibles |
|---|---|
| `filtroNombre` | Texto a buscar en el nombre |
| `ordenarPor` | `precio`, `-precio`, `stock`, `-stock`, `nombre`, `-nombre` |

### Categorías (`/api/v1/categorias`)

| Método | Ruta | Acción | Auth | Respuesta exitosa |
|---|---|---|---|---|
| `GET` | `/api/v1/categorias` | Lista categorías | API key | `200 OK` |
| `GET` | `/api/v1/categorias/{id}` | Obtiene por id | API key | `200 OK` |
| `POST` | `/api/v1/categorias` | Crea categoría | `admin` | `201 Created` |
| `PUT` | `/api/v1/categorias/{id}` | Actualiza categoría | `admin` | `204 No Content` |
| `DELETE` | `/api/v1/categorias/{id}` | Elimina categoría | `admin` | `204 No Content` |

## 7. Status codes utilizados

| Código | Significado | Cuándo se usa |
|---|---|---|
| `200 OK` | Éxito | `GET` exitoso. |
| `201 Created` | Recurso creado | `POST` exitoso. |
| `204 No Content` | Éxito sin body | `PUT`/`DELETE` exitoso. |
| `400 Bad Request` | Datos inválidos | DataAnnotations fallan. |
| `401 Unauthorized` | No autenticado | Falta `X-API-KEY` o es inválida. |
| `403 Forbidden` | Sin permisos | Key válida pero rol insuficiente. |
| `404 Not Found` | No encontrado | Producto o categoría no existe. |
| `500 Internal Server Error` | Error interno | Excepción no controlada. |

## 8. Flujo de una petición asíncrona

1. Request entra al middleware de excepciones.
2. Pasa por el filtro de autorización (`ApiKeyAuthorizationFilter`).
3. Routing dirige al controlador correspondiente.
4. Controller recibe `IProductoService` por constructor.
5. Controller invoca método `async` del servicio con `await`.
6. Servicio ejecuta operación asíncrona con EF Core (`ToListAsync`, `FirstOrDefaultAsync`, `SaveChangesAsync`).
7. Resultado sube al controller.
8. Controller devuelve `IActionResult` con el status code correspondiente.
9. Response sale al cliente.

## 9. Ejemplos de request y response

### Crear producto

```http
POST /api/v1/productos
Content-Type: application/json
X-API-KEY: admin-key

{
  "nombre": "Teclado mecánico",
  "precio": 150000,
  "stock": 10,
  "categoriaId": 1
}
```

Respuesta: `201 Created`

```json
{
  "id": 1,
  "nombre": "Teclado mecánico",
  "precio": 150000,
  "stock": 10,
  "categoriaId": 1
}
```

### Producto no encontrado

```http
GET /api/v1/productos/999
X-API-KEY: admin-key
```

Respuesta: `404 Not Found`

```json
{
  "mensaje": "No existe un producto con Id 999."
}
```

### Validación fallida

```http
POST /api/v1/productos
Content-Type: application/json
X-API-KEY: admin-key

{
  "nombre": "",
  "precio": -10,
  "stock": -5
}
```

Respuesta: `400 Bad Request`

```json
{
  "errors": {
    "Nombre": ["El nombre es obligatorio."],
    "Precio": ["El precio debe ser mayor que cero."],
    "Stock": ["El stock no puede ser negativo."]
  }
}
```

### Error interno

Respuesta genérica del middleware de excepciones:

```json
{
  "error": "Error interno del servidor",
  "detalle": "Ha ocurrido un error inesperado. Por favor, intente nuevamente.",
  "traceId": "0H1J2K3L4M5N6O7P8Q9R"
}
```

## 10. Consultas SQL

Archivo: `Sql/consultas.sql`

### Listar todos los productos

```sql
SELECT Id, Nombre, Precio, Stock, CategoriaId
FROM Productos;
```

### JOIN simple: productos con categoría

```sql
SELECT 
    p.Id AS ProductoId,
    p.Nombre AS ProductoNombre,
    p.Precio,
    p.Stock,
    c.Id AS CategoriaId,
    c.Nombre AS CategoriaNombre
FROM Productos p
INNER JOIN Categorias c ON p.CategoriaId = c.Id;
```

### Filtrar por nombre

```sql
SELECT Id, Nombre, Precio, Stock, CategoriaId
FROM Productos
WHERE Nombre LIKE '%teclado%';
```

### Ordenar por precio descendente

```sql
SELECT Id, Nombre, Precio, Stock, CategoriaId
FROM Productos
ORDER BY Precio DESC;
```

### Actualizar stock

```sql
UPDATE Productos
SET Stock = 20
WHERE Id = 1;
```

### Eliminar producto

```sql
DELETE FROM Productos
WHERE Id = 1;
```

### Contar productos por categoría

```sql
SELECT 
    c.Nombre AS Categoria,
    COUNT(p.Id) AS CantidadProductos
FROM Categorias c
LEFT JOIN Productos p ON c.Id = p.CategoriaId
GROUP BY c.Id, c.Nombre;
```

### Productos sin categoría

```sql
SELECT Id, Nombre, Precio, Stock
FROM Productos
WHERE CategoriaId IS NULL;
```

## 11. Procedimientos almacenados

SQLite no soporta procedimientos almacenados nativamente. A continuación se incluyen ejemplos en T-SQL para SQL Server.

### Obtener productos por categoría

```sql
CREATE PROCEDURE ObtenerProductosPorCategoria
    @CategoriaId INT
AS
BEGIN
    SELECT 
        p.Id,
        p.Nombre,
        p.Precio,
        p.Stock,
        c.Nombre AS CategoriaNombre
    FROM Productos p
    INNER JOIN Categorias c ON p.CategoriaId = c.Id
    WHERE p.CategoriaId = @CategoriaId;
END
GO
```

### Crear producto

```sql
CREATE PROCEDURE CrearProducto
    @Nombre NVARCHAR(100),
    @Precio DECIMAL(18,2),
    @Stock INT,
    @CategoriaId INT = NULL
AS
BEGIN
    INSERT INTO Productos (Nombre, Precio, Stock, CategoriaId)
    VALUES (@Nombre, @Precio, @Stock, @CategoriaId);

    SELECT * FROM Productos WHERE Id = last_insert_rowid();
END
GO
```

## 12. Ejecución local

### Requisitos

- SDK de .NET 10 instalado.
- Cliente HTTP como Postman, Bruno, Insomnia o `ApiProductos.http`.

### Comandos

```powershell
cd C:\Users\sebastian.macias\Desktop\ApiProductos\ApiProductos
dotnet restore
dotnet build
dotnet run
```

La API se ejecuta en:

```text
http://localhost:5021
```

Swagger UI:

```text
https://localhost:5021/swagger
```

## 13. Pruebas manuales

### Con archivo `.http`

Abrir `ApiProductos.http` en VS Code y ejecutar las peticiones en orden.

### Con PowerShell

```powershell
# Listar productos
Invoke-RestMethod -Uri "http://localhost:5021/api/v1/productos" -Method GET -Headers @{"x-api-key"="admin-key"}

# Crear producto
Invoke-RestMethod -Uri "http://localhost:5021/api/v1/productos" -Method POST -Headers @{"x-api-key"="admin-key"} -ContentType "application/json" -Body '{"nombre":"Teclado","precio":150000,"stock":10,"categoriaId":1}'

# Obtener producto por ID
Invoke-RestMethod -Uri "http://localhost:5021/api/v1/productos/1" -Method GET -Headers @{"x-api-key"="admin-key"}

# Actualizar producto
Invoke-RestMethod -Uri "http://localhost:5021/api/v1/productos/1" -Method PUT -Headers @{"x-api-key"="admin-key"} -ContentType "application/json" -Body '{"nombre":"Teclado RGB","precio":175000,"stock":8,"categoriaId":1}'

# Eliminar producto
Invoke-RestMethod -Uri "http://localhost:5021/api/v1/productos/1" -Method DELETE -Headers @{"x-api-key"="admin-key"}
```

## 14. Migraciones EF Core

```powershell
# Crear nueva migración
dotnet ef migrations add NombreDeLaMigracion

# Aplicar migraciones pendientes
dotnet ef database update

# Eliminar última migración (si no fue aplicada)
dotnet ef migrations remove
```

Archivos generados automáticamente:

- `Migrations/20260909212953_InitialCreate.cs`
- `Migrations/20260909222214_initialcreate2.cs`
- `Migrations/20260916202857_AgregarCategoria.cs`
- `Migrations/AppDbContextModelSnapshot.cs`

## 15. Estado actual

- CRUD completo de productos y categorías.
- Persistencia en SQLite con EF Core.
- Filtrado y ordenamiento con LINQ.
- Manejo global de errores y logs.
- Validaciones con DataAnnotations.
- Autenticación y autorización por API key.
- Documentación Swagger/OpenAPI.
- Versionamiento simple por URL (`/api/v1/...`).
- Repositorio sincronizado con rama `Macias` en GitHub.

## 16. Limitaciones y próximos pasos

1. Agregar DTOs de lectura separados para no exponer entidades directamente.
2. Implementar paginación en listados.
3. Agregar pruebas unitarias y de integración.
4. Migrar a SQL Server si se requiere mayor escalabilidad.
5. Agregar procedimientos almacenados si se migra a SQL Server.
6. Mejorar el filtro de autorización para soportar múltiples API keys desde configuración.
7. Agregar cache para consultas frecuentes.
8. Resolver advertencia de seguridad de `Microsoft.OpenApi` actualizando el paquete.
