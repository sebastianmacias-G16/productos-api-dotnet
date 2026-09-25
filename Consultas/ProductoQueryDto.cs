namespace ApiProductos.Consultas;

public class ProductoQueryDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public int? CategoriaId { get; set; }
}
