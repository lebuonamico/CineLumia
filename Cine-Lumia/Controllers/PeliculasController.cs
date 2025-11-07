using Microsoft.AspNetCore.Mvc;
using Cine_Lumia.Models;  
using Cine_Lumia.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Cine_Lumia.Controllers
{
    public class PeliculasController : Controller
    {
        private readonly CineDbContext _context;

        public PeliculasController(CineDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var peliculas = await _context.Peliculas
                .Include(p => p.PeliculaGeneros)
                .ThenInclude(pg => pg.Genero)
                .ToListAsync();
             
            //var peliculas = _context.Peliculas.ToList();
            return View(peliculas);
        }
    }
}