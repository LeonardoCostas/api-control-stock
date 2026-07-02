using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApipractica.contex;
using WebApipractica.models;

namespace WebApipractica.Controllers
{
    [ApiController]
    [Route("api/movimientos-stock")]
    public class MovimientosStockController : ControllerBase
    {
        private readonly Appdbcontex _context;

        public MovimientosStockController(Appdbcontex context)
        {
            _context = context;
        }

        [HttpPost("entrada")]
        public async Task<IActionResult> RegistrarEntrada(MovimientoStockDto dto)
        {
            if (dto.Cantidad <= 0)
                return BadRequest("La cantidad debe ser mayor a cero.");

            var producto = await BuscarProducto(dto.CodigoProducto, dto.AlmacenId);
            if (producto == null)
                return NotFound("No se encontro el producto en el almacen indicado.");

            producto.Stock += dto.Cantidad;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Tipo = "Entrada",
                producto.Codigo,
                producto.Nombre,
                dto.Cantidad,
                StockActual = producto.Stock,
                dto.Referencia,
                dto.Observacion
            });
        }

        [HttpPost("salida")]
        public async Task<IActionResult> RegistrarSalida(MovimientoStockDto dto)
        {
            if (dto.Cantidad <= 0)
                return BadRequest("La cantidad debe ser mayor a cero.");

            var producto = await BuscarProducto(dto.CodigoProducto, dto.AlmacenId);
            if (producto == null)
                return NotFound("No se encontro el producto en el almacen indicado.");

            if (producto.Stock < dto.Cantidad)
                return BadRequest($"Stock insuficiente. Disponible: {producto.Stock}.");

            producto.Stock -= dto.Cantidad;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Tipo = "Salida",
                producto.Codigo,
                producto.Nombre,
                dto.Cantidad,
                StockActual = producto.Stock,
                dto.Referencia,
                dto.Observacion
            });
        }

        [HttpPost("transferencia")]
        public async Task<IActionResult> TransferirEntreAlmacenes(TransferenciaStockDto dto)
        {
            if (dto.Cantidad <= 0)
                return BadRequest("La cantidad debe ser mayor a cero.");

            if (dto.AlmacenOrigenId == dto.AlmacenDestinoId)
                return BadRequest("El almacen origen y destino deben ser distintos.");

            var origen = await BuscarProducto(dto.CodigoProducto, dto.AlmacenOrigenId);
            if (origen == null)
                return NotFound("No se encontro el producto en el almacen origen.");

            if (origen.Stock < dto.Cantidad)
                return BadRequest($"Stock insuficiente en origen. Disponible: {origen.Stock}.");

            var destino = await BuscarProducto(dto.CodigoProducto, dto.AlmacenDestinoId);
            if (destino == null)
                return NotFound("Para transferir, primero debe existir el mismo codigo de producto en el almacen destino.");

            origen.Stock -= dto.Cantidad;
            destino.Stock += dto.Cantidad;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Tipo = "Transferencia",
                origen.Codigo,
                Producto = origen.Nombre,
                dto.Cantidad,
                AlmacenOrigenId = dto.AlmacenOrigenId,
                StockOrigen = origen.Stock,
                AlmacenDestinoId = dto.AlmacenDestinoId,
                StockDestino = destino.Stock,
                dto.Transporte,
                dto.Observacion
            });
        }

        private Task<Producto?> BuscarProducto(string codigo, int almacenId)
        {
            var codigoNormalizado = codigo.Trim().ToUpper();

            return _context.Productos
                .FirstOrDefaultAsync(producto =>
                    producto.Codigo.ToUpper() == codigoNormalizado &&
                    producto.AlmacenId == almacenId);
        }
    }
}
