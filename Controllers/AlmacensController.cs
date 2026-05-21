using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApipractica.contex;
using WebApipractica.models;

namespace WebApipractica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlmacenesController : ControllerBase
    {
        private readonly Appdbcontex _context;

        public AlmacenesController(Appdbcontex context)
        {
            _context = context;
        }

        // GET: api/almacenes
        [HttpGet]
        public async Task<IActionResult> GetAlmacenes()
        {
            var almacenes = await _context.Almacenes.ToListAsync();

            return Ok(almacenes);
        }

        // GET: api/almacenes/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlmacen(int id)
        {
            var almacen = await _context.Almacenes.FindAsync(id);

            if (almacen == null)
            {
                return NotFound();
            }

            return Ok(almacen);
        }

        // POST: api/almacenes
        [HttpPost]
        public async Task<IActionResult> CrearAlmacen(Almacen almacen)
        {
            _context.Almacenes.Add(almacen);

            await _context.SaveChangesAsync();

            return Ok(almacen);
        }
        // DELETE: api/almacenes/1
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EliminarAlmacen(int id)
        {
            var almacen = await _context.Almacenes.FindAsync(id);

            if (almacen == null)
                return NotFound();

            _context.Almacenes.Remove(almacen);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // PUT: api/almacenes/1
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarAlmacen(int id, Almacen almacen)
        {
            if (id != almacen.Id)
            {
                return BadRequest();
            }

            _context.Entry(almacen).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(almacen);
        }

        

    }
}