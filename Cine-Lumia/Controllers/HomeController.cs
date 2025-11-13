using Cine_Lumia.Models;
using Cine_Lumia.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cine_Lumia.Controllers
{
    public class HomeController : Controller
    {
        private readonly CineDbContext _context;
        private readonly IWebHostEnvironment _env;

        public HomeController(CineDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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

            // Leer banners desde wwwroot/images/banner
            try
            {
                var bannerDir = System.IO.Path.Combine(_env.ContentRootPath, "wwwroot", "images", "banner");
                if (System.IO.Directory.Exists(bannerDir))
                {
                    var files = System.IO.Directory.GetFiles(bannerDir)
                        .Where(f => new[] { ".png", ".jpg", ".jpeg", ".webp", ".gif" }
                        .Contains(System.IO.Path.GetExtension(f).ToLowerInvariant()))
                        .Select(f => "/images/banner/" + System.IO.Path.GetFileName(f))
                        .ToList();

                    viewModel.BannerUrls = files;
                }
            }
            catch
            {
                // ignorar errores de I/O
            }

            return View(viewModel);
        }
    }
}
