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

        [HttpGet]
        public IActionResult Seleccionar(int cineId)
        {
            TempData["CineSeleccionado"] = cineId;
            HttpContext.Session.SetInt32("CineSeleccionado", cineId);
            return Json(new { ok = true, cineId });
        }

        // Devuelve las salas (formatos) donde se exhibe una película. Si se pasa cineId, filtra por ese cine.
        [HttpGet]
        public async Task<IActionResult> SalasPorPelicula(int peliculaId, int? cineId)
        {
            // Primero intentamos encontrar salas (por sala) que tengan proyecciones para la película
            var salasQuery = _context.Salas
                .Include(s => s.Formato)
                .Where(s => _context.Proyecciones.Any(p => p.Id_Pelicula == peliculaId && p.Id_Sala == s.Id_Sala));

            if (cineId.HasValue)
            {
                salasQuery = salasQuery.Where(s => s.Id_Cine == cineId.Value);
            }

            var salas = await salasQuery
                .Select(s => new { IdSala = s.Id_Sala, Formato = s.Formato.Nombre })
                .Distinct()
                .ToListAsync();

            // Si no encontramos salas filtrando por cine, y se pidió un cine, hacer fallback a todas las salas donde exista la película (sin filtrar por cine)
            if ((salas == null || salas.Count == 0) && cineId.HasValue)
            {
                var salasFallback = await _context.Salas
                    .Include(s => s.Formato)
                    .Where(s => _context.Proyecciones.Any(p => p.Id_Pelicula == peliculaId && p.Id_Sala == s.Id_Sala))
                    .Select(s => new { IdSala = s.Id_Sala, Formato = s.Formato.Nombre })
                    .Distinct()
                    .ToListAsync();

                salas = salasFallback;
            }

            // Si aún no hay salas (posible si no hay proyecciones en la BD), devolver las distintas formatos existentes (por si se quiere mostrar opciones)
            if (salas == null || salas.Count == 0)
            {
                var formatos = await _context.Salas
                    .Include(s => s.Formato)
                    .Select(s => s.Formato.Nombre)
                    .Distinct()
                    .ToListAsync();

                var result = formatos.Select(f => new { IdSala = 0, Formato = f }).ToList();
                return Json(result);
            }

            return Json(salas);
        }
    }
}
