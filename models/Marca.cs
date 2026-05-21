namespace WebApipractica.models
{
    public class Marca
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Marca(int id , string Name)
        {
            this.Id = id;
            this.Name = Name;
        }
    }
}
