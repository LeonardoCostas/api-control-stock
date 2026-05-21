namespace WebApipractica.models
{
    public class TipoProducto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public TipoProducto(int Id , string Nombre)
        {
            this.Id = Id;
            this.Nombre = Nombre;
        }
    }
}
