using ApiProductos.Data;
using ApiProductos.Filters;
using ApiProductos.Middleware;
using ApiProductos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("MiHeaderPersonalizado", new OpenApiSecurityScheme
    {
        Name = "X-API-KEY",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "API key necesaria para acceder a los endpoints protegidos. Usa 'admin-key' o 'user-key'."
    });

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ApiProductos",
        Version = "v1",
        Description = "API REST para administrar productos y categorías",
        Contact = new OpenApiContact
        {
            Name = "Sebastian Macias",
            Email = "sebastian.macias@globant.com"
        }
    });

    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "ApiProductos.xml"));
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=productos.db"));

builder.Services.AddScoped<IProductoService, ProductoService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiProductos v1");
    options.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DataInitializer.Inicializar(db);
}

app.MapControllers();

app.Run();
