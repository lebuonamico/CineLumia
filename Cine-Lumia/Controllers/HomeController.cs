using Cine_Lumia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cine_Lumia.Controllers
{
    public class HomeController : Controller
    {
        private readonly CineDbContext _context;

        // Inyección de dependencia del DbContext
        public HomeController(CineDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // =====================
            // Cargar toda la información con relaciones
            // =====================

            // Empresas con sus Cines, Salas y Asientos
            var empresas = _context.Empresas
                .Include(e => e.Cines)
                    .ThenInclude(c => c.Salas)
                        .ThenInclude(s => s.Asientos)
                .ToList();

            // Peliculas con Géneros y Proyecciones con Entradas y Espectadores
            var peliculas = _context.Peliculas
                .Include(p => p.PeliculaGeneros)
                    .ThenInclude(pg => pg.Genero)
                .Include(p => p.Proyecciones)
                    .ThenInclude(pr => pr.Entradas)
                        .ThenInclude(en => en.Espectador)
                .ToList();

            // CineConsumibles
            var cineConsumibles = _context.CineConsumibles
                .Include(cc => cc.Cine)
                .Include(cc => cc.Consumible)
                .ToList();

            // Compras de Espectadores
            var compras = _context.EspectadorConsumibles
                .Include(ec => ec.Espectador)
                .Include(ec => ec.Cine)
                .Include(ec => ec.Consumible)
                .ToList();

            // Pasamos todo a un ViewModel
            var viewModel = new DashboardViewModel
            {
                Empresas = empresas,
                Peliculas = peliculas,
                CineConsumibles = cineConsumibles,
                Compras = compras
            };

            return View(viewModel);
        }
    }
}
