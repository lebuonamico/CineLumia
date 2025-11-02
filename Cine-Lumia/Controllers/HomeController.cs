using Cine_Lumia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Cine_Lumia.Controllers
{
    public class HomeController : Controller
    {
        private readonly CineDbContext _context;

        public HomeController(CineDbContext context)
        {
            _context = context;
        }

        // GET: / or /Home/Index
        public async Task<IActionResult> Index()
        {
            var peliculasQuery = _context.Peliculas
                .Include(p => p.PeliculaGeneros)
                .ThenInclude(pg => pg.Genero)
                .AsQueryable();

            var destacadas = await peliculasQuery
                .OrderByDescending(p => p.Fecha_Estreno)
                .Take(4)
                .ToListAsync();

            var cartelera = await peliculasQuery
                .OrderByDescending(p => p.Fecha_Estreno)
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                Destacadas = destacadas,
                Cartelera = cartelera
            };

            return View(viewModel);
        }
    }
}
