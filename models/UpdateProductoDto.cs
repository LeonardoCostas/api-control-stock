public class UpdateProductoDto
{
    public string Codigo { get; set; }
    public string Nombre { get; set; }
    public int Stock { get; set; }
    public decimal PrecioMayorista { get; set; }
    public int StockMinimo { get; set; } = 5;
    public string? ImagenUrl { get; set; }
    public bool Activo { get; set; } = true;

    public int AlmacenId { get; set; }
    public int MarcaId { get; set; }
    public int TipoProductoId { get; set; }
}
