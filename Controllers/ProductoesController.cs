using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetProductos()
        {
            var productos = await _context.Productos
                .Include(p => p.Almacen)
                .Include(p => p.Marca)
                .Include(p => p.TipoProducto)
                .Select(p => new
                {
                    p.Id,
                    p.Codigo,
                    p.Nombre,
                    p.Stock,
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
                .FirstOrDefaultAsync(p => p.Codigo == codigo);

            if (producto == null)
                return NotFound();

            return Ok(producto);
        }

        // POST: api/productos
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
                AlmacenId = dto.AlmacenId,
                MarcaId = dto.MarcaId,
                TipoProductoId = dto.TipoProductoId
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return Ok(producto);
        }

        // PUT: api/productos/1
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarProducto(int id, CreateProductoDto dto)
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
            producto.AlmacenId = dto.AlmacenId;
            producto.MarcaId = dto.MarcaId;
            producto.TipoProductoId = dto.TipoProductoId;

            await _context.SaveChangesAsync();

            return Ok(producto);
        }

        // DELETE: api/productos/1
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