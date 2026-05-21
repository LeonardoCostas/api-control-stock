namespace WebApipractica.models
{
    public class Producto
    {
        public int Id { get; set; }

        public required string Codigo { get; set; }

        public required string Nombre { get; set; }

        public required int Stock { get; set; }


        // Foreign Keys
        public required int AlmacenId { get; set; }

        public required int MarcaId { get; set; }

        public required int TipoProductoId { get; set; }


        // Navigation Properties
        public Almacen  Almacen { get; set; }

        public Marca Marca { get; set; }

        public TipoProducto TipoProducto { get; set; }
    }
}
