using Microsoft.EntityFrameworkCore;
using WebApipractica.models;

namespace WebApipractica.contex
{
    public class Appdbcontex :DbContext

    {
        public Appdbcontex(DbContextOptions<Appdbcontex>options):base(options)
        {
            
        }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Almacen> Almacenes { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<TipoProducto> TipoProducto { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Producto>()
                .Property(producto => producto.PrecioMayorista)
                .HasColumnType("decimal(18,2)");
        }
    }

}
