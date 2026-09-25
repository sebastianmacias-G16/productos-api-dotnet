namespace ApiProductos.Consultas;

public class ProductoConCategoriaQueryDto
{
    public int ProductoId { get; set; }
    public string Producto { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public int CategoriaId { get; set; }
    public string Categoria { get; set; } = string.Empty;
}
