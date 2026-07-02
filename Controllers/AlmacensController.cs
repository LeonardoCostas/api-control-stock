using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApipractica.contex;
using WebApipractica.models;

namespace WebApipractica.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AlmacenesController : ControllerBase
    {
        private readonly Appdbcontex _context;

        public AlmacenesController(Appdbcontex context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAlmacenes()
        {
            return Ok(await _context.Almacenes.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlmacen(int id)
        {
            var almacen = await _context.Almacenes.FindAsync(id);

            if (almacen == null)
                return NotFound();

            return Ok(almacen);
        }

        [HttpPost]
        public async Task<IActionResult> CrearAlmacen(Almacen almacen)
        {
            _context.Almacenes.Add(almacen);
            await _context.SaveChangesAsync();

            return Ok(almacen);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarAlmacen(int id)
        {
            var almacen = await _context.Almacenes.FindAsync(id);

            if (almacen == null)
                return NotFound();

            _context.Almacenes.Remove(almacen);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarAlmacen(int id, Almacen almacen)
        {
            if (id != almacen.Id)
                return BadRequest();

            _context.Entry(almacen).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(almacen);
        }
    }
}
