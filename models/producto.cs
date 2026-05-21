namespace WebApipractica.models
{
    public class Producto
    {
        public int Id { get; set; }

        public string Codigo { get; set; }

        public string Nombre { get; set; }

        public int Stock { get; set; }


        // Foreign Keys
        public int AlmacenId { get; set; }

        public int MarcaId { get; set; }

        public int TipoProductoId { get; set; }


        // Navigation Properties
        public Almacen Almacen { get; set; }

        public Marca Marca { get; set; }

        public TipoProducto TipoProducto { get; set; }
    }
}
