using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApipractica.contex;

namespace WebApipractica.Controllers
{
    [ApiController]
    [Route("api/reportes")]
    public class ReportesController : ControllerBase
    {
        private readonly Appdbcontex _context;

        public ReportesController(Appdbcontex context)
        {
            _context = context;
        }

        [HttpGet("resumen")]
        public async Task<IActionResult> ObtenerResumen()
        {
            var productos = await _context.Productos
                .Include(producto => producto.Almacen)
                .Include(producto => producto.Marca)
                .Include(producto => producto.TipoProducto)
                .Where(producto => producto.Activo)
                .ToListAsync();

            var resumenDepositos = productos
                .GroupBy(producto => producto.Almacen.Nombre)
                .Select(grupo => new
                {
                    Almacen = grupo.Key,
                    Productos = grupo.Count(),
                    Unidades = grupo.Sum(producto => producto.Stock)
                })
                .OrderBy(item => item.Almacen);

            var resumenCategorias = productos
                .GroupBy(producto => producto.TipoProducto.Nombre)
                .Select(grupo => new
                {
                    Categoria = grupo.Key,
                    Productos = grupo.Count(),
                    Unidades = grupo.Sum(producto => producto.Stock)
                })
                .OrderByDescending(item => item.Unidades);

            return Ok(new
            {
                TotalProductos = productos.Count,
                TotalUnidades = productos.Sum(producto => producto.Stock),
                ValorInventarioMayorista = productos.Sum(producto => producto.Stock * producto.PrecioMayorista),
                Depositos = resumenDepositos,
                Categorias = resumenCategorias
            });
        }

        [HttpGet("stock-bajo")]
        public async Task<IActionResult> ObtenerStockBajo([FromQuery] int? minimo = null)
        {
            if (minimo.HasValue && minimo.Value < 0)
                return BadRequest("El minimo no puede ser negativo.");

            var productos = await _context.Productos
                .Include(producto => producto.Almacen)
                .Include(producto => producto.Marca)
                .Include(producto => producto.TipoProducto)
                .Where(producto => producto.Activo && producto.Stock <= (minimo ?? producto.StockMinimo))
                .OrderBy(producto => producto.Stock)
                .Select(producto => new
                {
                    producto.Id,
                    producto.Codigo,
                    producto.Nombre,
                    producto.Stock,
                    producto.StockMinimo,
                    producto.PrecioMayorista,
                    producto.ImagenUrl,
                    Almacen = producto.Almacen.Nombre,
                    Marca = producto.Marca.Name,
                    TipoProducto = producto.TipoProducto.Nombre
                })
                .ToListAsync();

            return Ok(productos);
        }
    }
}
