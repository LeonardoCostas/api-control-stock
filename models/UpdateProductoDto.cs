public class UpdateProductoDto
{
    public string Codigo { get; set; }
    public string Nombre { get; set; }
    public int Stock { get; set; }

    public int AlmacenId { get; set; }
    public int MarcaId { get; set; }
    public int TipoProductoId { get; set; }
}
