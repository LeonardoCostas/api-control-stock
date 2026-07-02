namespace WebApipractica.models
{
    public class MovimientoStockDto
    {
        public required string CodigoProducto { get; set; }
        public int AlmacenId { get; set; }
        public int Cantidad { get; set; }
        public string? Referencia { get; set; }
        public string? Observacion { get; set; }
    }

    public class TransferenciaStockDto
    {
        public required string CodigoProducto { get; set; }
        public int AlmacenOrigenId { get; set; }
        public int AlmacenDestinoId { get; set; }
        public int Cantidad { get; set; }
        public string? Transporte { get; set; }
        public string? Observacion { get; set; }
    }
}
