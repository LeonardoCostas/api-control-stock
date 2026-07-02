using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApipractica.contex;
using WebApipractica.models;

namespace WebApipractica.Controllers
{
    [ApiController]
    [Route("api/datos-demo")]
    public class DatosDemoController : ControllerBase
    {
        private readonly Appdbcontex _context;

        public DatosDemoController(Appdbcontex context)
        {
            _context = context;
        }

        [HttpPost("seed")]
        public async Task<IActionResult> CrearDatosDemo()
        {
            if (await _context.Productos.AnyAsync())
                return Ok("La base ya tiene productos cargados.");

            var principal = new Almacen(0, "Deposito Barracas", "Herrera 1171, Barracas");
            var reparto = new Almacen(0, "Deposito Reparto", "Preparacion de pedidos");

            var shimano = new Marca(0, "Shimano");
            var generica = new Marca(0, "Zamponi");
            var kenda = new Marca(0, "Kenda");
            var promax = new Marca(0, "Promax");

            var transmision = new TipoProducto(0, "Transmision");
            var frenos = new TipoProducto(0, "Frenos");
            var ruedas = new TipoProducto(0, "Ruedas");
            var accesorios = new TipoProducto(0, "Accesorios");
            var seguridad = new TipoProducto(0, "Seguridad");

            _context.Almacenes.AddRange(principal, reparto);
            _context.Marcas.AddRange(shimano, generica, kenda, promax);
            _context.TipoProducto.AddRange(transmision, frenos, ruedas, accesorios, seguridad);
            await _context.SaveChangesAsync();

            var productos = new List<Producto>
            {
                new() { Codigo = "CAD-116-SHI", Nombre = "Cadena Shimano 116 eslabones", Stock = 42, AlmacenId = principal.Id, MarcaId = shimano.Id, TipoProductoId = transmision.Id },
                new() { Codigo = "PIN-7V-INDEX", Nombre = "Pinon indexado 7 velocidades", Stock = 18, AlmacenId = principal.Id, MarcaId = generica.Id, TipoProductoId = transmision.Id },
                new() { Codigo = "PUN-ERG-NEG", Nombre = "Punos ergonomicos negros", Stock = 96, AlmacenId = principal.Id, MarcaId = generica.Id, TipoProductoId = accesorios.Id },
                new() { Codigo = "CAR-750-TRA", Nombre = "Caramanola transparente 750 ml", Stock = 130, AlmacenId = principal.Id, MarcaId = generica.Id, TipoProductoId = accesorios.Id },
                new() { Codigo = "BOT-ALU-LAT", Nombre = "Porta botella aluminio lateral", Stock = 54, AlmacenId = reparto.Id, MarcaId = generica.Id, TipoProductoId = accesorios.Id },
                new() { Codigo = "CUB-29-KEN", Nombre = "Cubierta MTB 29 x 2.10", Stock = 27, AlmacenId = principal.Id, MarcaId = kenda.Id, TipoProductoId = ruedas.Id },
                new() { Codigo = "CAM-26-VAL", Nombre = "Camara 26 valvula auto", Stock = 8, AlmacenId = reparto.Id, MarcaId = generica.Id, TipoProductoId = ruedas.Id },
                new() { Codigo = "FRE-DIS-PMX", Nombre = "Caliper freno a disco mecanico", Stock = 14, AlmacenId = principal.Id, MarcaId = promax.Id, TipoProductoId = frenos.Id },
                new() { Codigo = "PAS-DIS-ORG", Nombre = "Pastillas de freno organicas", Stock = 6, AlmacenId = reparto.Id, MarcaId = generica.Id, TipoProductoId = frenos.Id },
                new() { Codigo = "CAS-MTB-MED", Nombre = "Casco MTB regulable mediano", Stock = 21, AlmacenId = principal.Id, MarcaId = generica.Id, TipoProductoId = seguridad.Id }
            };

            _context.Productos.AddRange(productos);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Mensaje = "Datos demo cargados.",
                Productos = productos.Count
            });
        }
    }
}
