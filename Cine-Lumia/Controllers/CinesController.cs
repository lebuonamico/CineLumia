using Cine_Lumia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cine_Lumia.Controllers
{
    public class CinesController : Controller
    {
        private readonly CineDbContext _context;

        public CinesController(CineDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var cines = await _context.Cines
                .Include(c => c.Empresa)
                .Select(c => new
                {
                    id = c.Id_Cine,
                    nombre = c.Nombre,
                    direccion = c.Direccion,
                    empresa = c.Empresa.Nombre
                })
                .ToListAsync();

            return Json(cines);
        }
    }
}
