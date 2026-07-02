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
    public class MarcasController : ControllerBase
    {
        private readonly Appdbcontex _context;

        public MarcasController(Appdbcontex context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMarcas()
        {
            return Ok(await _context.Marcas.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMarca(int id)
        {
            var marca = await _context.Marcas.FindAsync(id);

            if (marca == null)
                return NotFound();

            return Ok(marca);
        }

        [HttpPost]
        public async Task<IActionResult> CrearMarca(Marca marca)
        {
            _context.Marcas.Add(marca);
            await _context.SaveChangesAsync();

            return Ok(marca);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarMarca(int id, Marca marca)
        {
            if (id != marca.Id)
                return BadRequest();

            _context.Entry(marca).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(marca);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarMarca(int id)
        {
            var marca = await _context.Marcas.FindAsync(id);

            if (marca == null)
                return NotFound();

            _context.Marcas.Remove(marca);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
