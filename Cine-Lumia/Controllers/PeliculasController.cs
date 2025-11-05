using Microsoft.AspNetCore.Mvc;
using Cine_Lumia.Models;  
using System.Linq;

namespace Cine_Lumia.Controllers
{
    public class PeliculasController : Controller
    {
        private readonly CineDbContext _context;

        public PeliculasController(CineDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
             
            var peliculas = _context.Peliculas.ToList();
            return View(peliculas);
        }
    }
}