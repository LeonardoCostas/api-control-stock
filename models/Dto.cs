public class CreateProductoDto
{
    public required string Codigo { get; set; }
    public required string Nombre { get; set; }
    public required int Stock { get; set; }
    public decimal PrecioMayorista { get; set; }
    public int StockMinimo { get; set; } = 5;
    public string? ImagenUrl { get; set; }

    public int AlmacenId { get; set; }
    public int MarcaId { get; set; }
    public int TipoProductoId { get; set; }
}
