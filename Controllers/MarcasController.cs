using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApipractica.contex;
using WebApipractica.models;

namespace WebApipractica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarcasController : ControllerBase
    {
        private readonly Appdbcontex _context;

        public MarcasController(Appdbcontex context)
        {
            _context = context;
        }

        // GET: api/marcas
        [HttpGet]
        public async Task<IActionResult> GetMarcas()
        {
            var marcas = await _context.Marcas.ToListAsync();

            return Ok(marcas);
        }

        // GET: api/marcas/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMarca(int id)
        {
            var marca = await _context.Marcas.FindAsync(id);

            if (marca == null)
            {
                return NotFound();
            }

            return Ok(marca);
        }

        // POST: api/marcas
        [HttpPost]
        public async Task<IActionResult> CrearMarca(Marca marca)
        {
            _context.Marcas.Add(marca);

            await _context.SaveChangesAsync();

            return Ok(marca);
        }

        // PUT: api/marcas/1
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarMarca(int id, Marca marca)
        {
            if (id != marca.Id)
            {
                return BadRequest();
            }

            _context.Entry(marca).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(marca);
        }

        // DELETE: api/marcas/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarMarca(int id)
        {
            var marca = await _context.Marcas.FindAsync(id);

            if (marca == null)
            {
                return NotFound();
            }

            _context.Marcas.Remove(marca);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}