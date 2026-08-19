# ApiProductos — Documentación del proyecto

## 1. Propósito

**ApiProductos** es una Web API REST desarrollada con ASP.NET Core (.NET 10) para administrar productos. Implementa las operaciones CRUD sobre una colección en memoria, por lo que no requiere base de datos para funcionar.

El proyecto sirve como práctica de:

- Desarrollo de una API REST con ASP.NET Core.
- Uso de controladores, rutas y métodos HTTP.
- Separación entre entidades y DTOs.
- Validación automática de solicitudes.
- Uso de Git para el control de versiones.

> Los datos existen únicamente mientras la aplicación está ejecutándose. Al detenerla, la lista de productos se reinicia.

## 2. Tecnologías

| Tecnología | Uso en el proyecto |
|---|---|
| .NET 10 | Plataforma de ejecución y desarrollo. |
| ASP.NET Core | Framework para crear la API HTTP. |
| C# | Lenguaje de programación. |
| OpenAPI | Exposición del documento de la API en entorno de desarrollo. |
| Git | Control de versiones local. |

## 3. Estructura del proyecto

```text
ApiProductos/
├── Controllers/
│   └── ProductosController.cs
├── DTOs/
│   └── CrearProductoDto.cs
├── Models/
│   └── Producto.cs
├── Properties/
│   └── launchSettings.json
├── ApiProductos.http
├── ApiProductos.csproj
├── appsettings.Development.json
├── appsettings.json
├── DOCUMENTACION.md
└── Program.cs
```

## 4. Arquitectura y responsabilidades

### `Program.cs`

Es el punto de entrada de la aplicación. Registra los servicios y configura el pipeline HTTP:

```csharp
builder.Services.AddControllers();
builder.Services.AddOpenApi();

app.UseHttpsRedirection();
app.MapControllers();
```

- `AddControllers()` habilita el uso de controladores MVC para atender solicitudes HTTP.
- `AddOpenApi()` registra el soporte de OpenAPI.
- `UseHttpsRedirection()` redirige solicitudes HTTP a HTTPS cuando existe un puerto HTTPS configurado.
- `MapControllers()` publica las rutas definidas en los controladores.

### `Models/Producto.cs`

Representa la entidad de negocio almacenada en memoria.

| Propiedad | Tipo | Descripción |
|---|---|---|
| `Id` | `int` | Identificador único autoincrementable. |
| `Nombre` | `string` | Nombre del producto. |
| `Precio` | `decimal` | Valor monetario del producto. |
| `Stock` | `int` | Unidades disponibles. |

### `DTOs/CrearProductoDto.cs`

Representa los datos que un cliente puede enviar al crear o actualizar un producto. No contiene `Id`, ya que dicho valor lo controla el servidor.

Incluye las siguientes validaciones:

| Campo | Regla |
|---|---|
| `Nombre` | Obligatorio y con máximo 100 caracteres. |
| `Precio` | Debe ser mayor que cero. |
| `Stock` | No puede ser negativo. |

El atributo `[ApiController]` activa la validación automática. Si un JSON no cumple las reglas, ASP.NET Core responde con **400 Bad Request** sin entrar al método del controlador.

### `Controllers/ProductosController.cs`

Contiene la lógica de los endpoints de productos.

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
```

La ruta `[Route("api/[controller]")]` usa el nombre del controlador sin el sufijo `Controller`; por tanto, `ProductosController` se publica en `/api/productos`.

La persistencia se simula con estos campos estáticos:

```csharp
private static readonly List<Producto> Productos = new();
private static int siguienteId = 1;
```

- `Productos` almacena los productos en memoria.
- `siguienteId` genera identificadores consecutivos para cada producto creado.

## 5. Conceptos HTTP usados

| Concepto | Aplicación en ApiProductos |
|---|---|
| Recurso | Un producto. |
| Colección de recursos | `/api/productos`. |
| Recurso individual | `/api/productos/{id}`. |
| `[FromRoute]` | Lee `id` desde la URL, por ejemplo `/api/productos/1`. |
| `[FromBody]` | Convierte el JSON enviado por el cliente en `CrearProductoDto`. |
| Model binding | Proceso automático de ASP.NET Core para enlazar datos HTTP con parámetros C#. |

## 6. Endpoints disponibles

| Método | Ruta | Acción | Respuesta exitosa |
|---|---|---|---|
| `GET` | `/api/productos` | Obtiene todos los productos. | `200 OK` |
| `GET` | `/api/productos/{id}` | Busca un producto por identificador. | `200 OK` |
| `POST` | `/api/productos` | Crea un producto. | `201 Created` |
| `PUT` | `/api/productos/{id}` | Actualiza todos los datos editables de un producto. | `204 No Content` |
| `DELETE` | `/api/productos/{id}` | Elimina un producto. | `204 No Content` |

Cuando se solicita, modifica o elimina un identificador inexistente, la API devuelve:

```http
404 Not Found
```

```json
{
  "mensaje": "No existe un producto con Id 999."
}
```

## 7. Flujo de una creación

1. El cliente envía un `POST /api/productos` con JSON.
2. ASP.NET Core transforma el body en un objeto `CrearProductoDto`.
3. Las reglas de validación verifican los datos.
4. El controlador crea un objeto `Producto` y asigna un nuevo `Id`.
5. El producto se agrega a la lista en memoria.
6. La API responde `201 Created`, con el producto y el encabezado `Location` que apunta al nuevo recurso.

Ejemplo de respuesta:

```json
{
  "id": 1,
  "nombre": "Teclado mecánico",
  "precio": 150000,
  "stock": 10
}
```

## 8. Ejecución local

### Requisitos

- SDK de .NET 10 instalado.
- Un cliente HTTP como Postman, Bruno, Insomnia o el archivo `.http` compatible con Visual Studio/VS Code.

### Comandos

Desde la raíz del proyecto:

```powershell
dotnet restore
dotnet build
dotnet run
```

El perfil actual de desarrollo está configurado para ejecutarse en:

```text
http://localhost:5021
```

La URL definitiva siempre se muestra en la consola al ejecutar `dotnet run`.

## 9. Pruebas manuales

Define esta URL base en Postman:

```text
http://localhost:5021
```

Para solicitudes con JSON, usa el header:

```http
Content-Type: application/json
```

### Crear producto

```http
POST /api/productos
```

```json
{
  "nombre": "Teclado mecánico",
  "precio": 150000,
  "stock": 10
}
```

Respuesta esperada: `201 Created`.

### Listar productos

```http
GET /api/productos
```

Respuesta esperada: `200 OK`.

### Consultar producto por ID

```http
GET /api/productos/1
```

Respuesta esperada: `200 OK`, o `404 Not Found` si no existe.

### Actualizar producto

```http
PUT /api/productos/1
```

```json
{
  "nombre": "Teclado mecánico RGB",
  "precio": 175000,
  "stock": 8
}
```

Respuesta esperada: `204 No Content`, o `404 Not Found` si no existe.

### Eliminar producto

```http
DELETE /api/productos/1
```

Respuesta esperada: `204 No Content`, o `404 Not Found` si no existe.

También puedes ejecutar estas mismas solicitudes desde `ApiProductos.http`; se deben ejecutar en el orden: crear, listar/consultar, actualizar y eliminar.

## 10. Validaciones y errores

Ejemplo de solicitud inválida:

```json
{
  "nombre": "",
  "precio": 0,
  "stock": -1
}
```

La API responde `400 Bad Request` con detalles de los campos que no cumplen las reglas.

## 11. Control de versiones con Git

El repositorio local ya fue inicializado y contiene un `.gitignore` para .NET. Este archivo evita incluir directorios generados, como `bin/` y `obj/`, en los commits.

Flujo recomendado:

```powershell
git status
git add .
git commit -m "feat: implementar CRUD de productos en memoria"
```

Mensajes de commit sugeridos para la evolución del proyecto:

```text
chore: inicializar proyecto Web API y configurar gitignore
feat: agregar modelo Producto
feat: agregar DTO de productos con validaciones
feat: implementar CRUD de productos en memoria
docs: agregar documentación y ejemplos de pruebas HTTP
```

Para publicar el repositorio en una plataforma remota:

```powershell
git branch -M main
git remote add origin URL_DEL_REPOSITORIO
git push -u origin main
```

## 12. Limitaciones actuales y siguientes pasos

Esta implementación es intencionalmente sencilla y educativa. Para convertirla en una aplicación más cercana a producción, los siguientes pasos recomendados son:

1. Reemplazar la lista estática por una base de datos, por ejemplo SQL Server con Entity Framework Core.
2. Crear DTOs de lectura, como `ProductoDto`, para no devolver la entidad de dominio directamente.
3. Agregar pruebas unitarias y de integración.
4. Añadir logging, manejo centralizado de errores y respuestas de error estandarizadas.
5. Agregar autenticación y autorización si la API maneja usuarios reales.
6. Documentar formalmente los endpoints mediante Swagger UI u OpenAPI.
7. Resolver la advertencia de seguridad de la dependencia `Microsoft.OpenApi` actualizando a una versión compatible que no incluya la vulnerabilidad reportada por NuGet.

## 13. Estado actual

La API cuenta con el CRUD requerido, validación de entrada, persistencia temporal en memoria, rutas REST y ejemplos de solicitudes. El proyecto fue compilado correctamente después de implementar estos cambios.
