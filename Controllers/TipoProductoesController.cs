using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApipractica.contex;
using WebApipractica.models;

namespace WebApipractica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipoProductosController : ControllerBase
    {
        private readonly Appdbcontex _context;

        public TipoProductosController(Appdbcontex context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTipos()
        {
            return Ok(await _context.TipoProducto.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CrearTipo(TipoProducto tipo)
        {
            _context.TipoProducto.Add(tipo);

            await _context.SaveChangesAsync();

            return Ok(tipo);
        }
    }
}
