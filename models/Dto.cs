public class CreateProductoDto
{
    public required string Codigo { get; set; }
    public required string Nombre { get; set; }
    public required int Stock { get; set; }

    public int AlmacenId { get; set; }
    public int MarcaId { get; set; }
    public int TipoProductoId { get; set; }
}