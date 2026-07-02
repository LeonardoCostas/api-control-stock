using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WebApipractica.contex;
using WebApipractica.models;

namespace WebApipractica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly Appdbcontex _context;

        public ProductosController(Appdbcontex context)
        {
            _context = context;
        }

        // GET: api/productos
        [HttpGet]
        public async Task<IActionResult> GetProductos([FromQuery] bool incluirInactivos = false)
        {
            var productos = await _context.Productos
                .Include(p => p.Almacen)
                .Include(p => p.Marca)
                .Include(p => p.TipoProducto)
                .Where(p => incluirInactivos || p.Activo)
                .Select(p => new
                {
                    p.Id,
                    p.Codigo,
                    p.Nombre,
                    p.Stock,
                    p.PrecioMayorista,
                    p.StockMinimo,
                    p.ImagenUrl,
                    p.Activo,
                    p.AlmacenId,
                    p.MarcaId,
                    p.TipoProductoId,
                    Almacen = p.Almacen.Nombre,
                    Marca = p.Marca.Name,
                    TipoProducto = p.TipoProducto.Nombre
                })
                .ToListAsync();

            return Ok(productos);
        }

        // GET: api/productos/1
        [HttpGet("codigo/{codigo}")]
        public async Task<IActionResult> GetProductoPorCodigo(string codigo)
        {
            var producto = await _context.Productos
                .Include(p => p.Almacen)
                .Include(p => p.Marca)
                .Include(p => p.TipoProducto)
                .FirstOrDefaultAsync(p => p.Activo && p.Codigo.ToUpper() == codigo.Trim().ToUpper());

            if (producto == null)
                return NotFound();

            return Ok(producto);
        }

        // GET: api/productos/buscar?texto=CAD-116
        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarProductos([FromQuery] string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return BadRequest("Ingrese un codigo, nombre, marca o categoria.");

            var busqueda = texto.Trim().ToUpper();

            var productos = await _context.Productos
                .Include(p => p.Almacen)
                .Include(p => p.Marca)
                .Include(p => p.TipoProducto)
                .Where(p =>
                    p.Activo &&
                    (p.Codigo.ToUpper().Contains(busqueda) ||
                    p.Nombre.ToUpper().Contains(busqueda) ||
                    p.Marca.Name.ToUpper().Contains(busqueda) ||
                    p.TipoProducto.Nombre.ToUpper().Contains(busqueda)))
                .OrderByDescending(p => p.Codigo.ToUpper() == busqueda)
                .ThenBy(p => p.Codigo)
                .Select(p => new
                {
                    p.Id,
                    p.Codigo,
                    p.Nombre,
                    p.Stock,
                    p.PrecioMayorista,
                    p.StockMinimo,
                    p.ImagenUrl,
                    p.Activo,
                    AlmacenId = p.AlmacenId,
                    MarcaId = p.MarcaId,
                    TipoProductoId = p.TipoProductoId,
                    Almacen = p.Almacen.Nombre,
                    Marca = p.Marca.Name,
                    TipoProducto = p.TipoProducto.Nombre
                })
                .Take(25)
                .ToListAsync();

            return Ok(productos);
        }

        // POST: api/productos
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CrearProducto(CreateProductoDto dto)
        {
            // ✔ Validar Almacén
            if (!await _context.Almacenes.AnyAsync(x => x.Id == dto.AlmacenId))
                return BadRequest("Almacén no existe");

            // ✔ Validar Marca
            if (!await _context.Marcas.AnyAsync(x => x.Id == dto.MarcaId))
                return BadRequest("Marca no existe");

            // ✔ Validar TipoProducto
            if (!await _context.TipoProducto.AnyAsync(x => x.Id == dto.TipoProductoId))
                return BadRequest("Tipo de producto no existe");

            var producto = new Producto
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                Stock = dto.Stock,
                PrecioMayorista = dto.PrecioMayorista,
                StockMinimo = dto.StockMinimo,
                ImagenUrl = dto.ImagenUrl,
                Activo = true,
                AlmacenId = dto.AlmacenId,
                MarcaId = dto.MarcaId,
                TipoProductoId = dto.TipoProductoId
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return Ok(producto);
        }

        // PUT: api/productos/1
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarProducto(int id, UpdateProductoDto dto)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return NotFound();

            if (!await _context.Almacenes.AnyAsync(x => x.Id == dto.AlmacenId))
                return BadRequest("Almacén no existe");

            if (!await _context.Marcas.AnyAsync(x => x.Id == dto.MarcaId))
                return BadRequest("Marca no existe");

            if (!await _context.TipoProducto.AnyAsync(x => x.Id == dto.TipoProductoId))
                return BadRequest("TipoProducto no existe");

            producto.Codigo = dto.Codigo;
            producto.Nombre = dto.Nombre;
            producto.Stock = dto.Stock;
            producto.PrecioMayorista = dto.PrecioMayorista;
            producto.StockMinimo = dto.StockMinimo;
            producto.ImagenUrl = dto.ImagenUrl;
            producto.Activo = dto.Activo;
            producto.AlmacenId = dto.AlmacenId;
            producto.MarcaId = dto.MarcaId;
            producto.TipoProductoId = dto.TipoProductoId;

            await _context.SaveChangesAsync();

            return Ok(producto);
        }

        // PATCH: api/productos/1/desactivar
        [Authorize]
        [HttpPatch("{id}/desactivar")]
        public async Task<IActionResult> DesactivarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return NotFound();

            producto.Activo = false;
            await _context.SaveChangesAsync();

            return Ok(producto);
        }

        // DELETE: api/productos/1
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            _context.Productos.Remove(producto);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
