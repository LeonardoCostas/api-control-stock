public class ProductoResponseDto
{
    public int Id { get; set; }
    public required string Codigo { get; set; }
    public required string Nombre { get; set; }
    public int Stock { get; set; }

    public required string Almacen { get; set; }
    public required string Marca { get; set; }
    public required string TipoProducto { get; set; }
}