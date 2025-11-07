using Cine_Lumia.Models;
using Cine_Lumia.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Cine_Lumia.Controllers
{
    public class VentaEntradasController : Controller
    {
        private readonly CineDbContext _context;
        public VentaEntradasController(CineDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int idProyeccion)
        {
            var proyeccion = await _context.Proyecciones
                .Include(p => p.Sala)
                    .ThenInclude(s => s.Formato)
                .Include(p => p.Pelicula)
                .Include(p => p.Sala.Cine)
                .FirstOrDefaultAsync(p => p.Id_Proyeccion == idProyeccion);

            if (proyeccion == null)
                return NotFound();

            // Traer los tipos de entrada según formato
            var tiposEntrada = await _context.TipoEntrada
                .Where(t => t.Id_Formato == proyeccion.Sala.Id_Formato)
                .ToListAsync();

            // 🔸 Calcular asientos disponibles para esta proyección
            var totalAsientos = proyeccion.Sala.Capacidad;
            var entradasVendidas = await _context.Entradas.CountAsync(e => e.Id_Proyeccion == idProyeccion);
            var asientosDisponibles = totalAsientos - entradasVendidas;

            var vm = new VentaEntradasViewModel
            {
                Proyeccion = proyeccion,
                TiposEntrada = tiposEntrada,
                TotalAsientos = totalAsientos,
                AsientosDisponibles = asientosDisponibles
            };

            return View(vm);
        }

    }
}
