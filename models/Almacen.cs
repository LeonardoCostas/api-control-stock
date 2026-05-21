namespace WebApipractica.models
{
    public class Almacen
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }

        public Almacen(int id , string Nombre,string Direccion)
        {
            this.Id = id;
            this.Nombre = Nombre;
            this.Direccion = Direccion;
            
        }
    }
}
