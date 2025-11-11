using Cine_Lumia.Models;
using Cine_Lumia.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cine_Lumia.Controllers
{
    public class FuncionesController : Controller
    {
        private readonly CineDbContext _context;

        public FuncionesController(CineDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int cineId, int peliculaId, string? fecha)
        {
            var cine = await _context.Cines.FirstOrDefaultAsync(c => c.Id_Cine == cineId);
            var pelicula = await _context.Peliculas.FirstOrDefaultAsync(p => p.Id_Pelicula == peliculaId);

            if (cine == null || pelicula == null)
                return NotFound();

            bool desdeVenta = TempData.Peek("DesdeVentaEntradas") != null;
            Console.WriteLine($"🔹 Funciones.Index - desdeVenta = {desdeVenta}");

            if (!desdeVenta)
            {
                Console.WriteLine("🧹 Entrada directa: limpiando TempData viejo...");
                TempData.Clear();
            }

            var fechas = Enumerable.Range(0, 7)
                .Select(d => DateTime.Today.AddDays(d).ToString("yyyy-MM-dd"))
                .ToList();

            string fechaSeleccionada = fecha ?? DateTime.Today.ToString("yyyy-MM-dd");
            var ahora = DateTime.Now;

            var proyeccionesList = await _context.Proyecciones
                .Include(p => p.Sala)
                    .ThenInclude(s => s.Formato)
                .Where(p => p.Id_Pelicula == peliculaId && p.Sala.Id_Cine == cineId)
                .ToListAsync();

            proyeccionesList = proyeccionesList
                .Where(p =>
                    p.Fecha.Date > DateTime.Today ||
                    (p.Fecha.Date == DateTime.Today && p.Hora > ahora.TimeOfDay))
                .ToList();

            var proyeccionesPorFecha = proyeccionesList
                .GroupBy(p => p.Fecha.Date.ToString("yyyy-MM-dd"))
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(p => p.Sala)
                          .Select(s => new SalaConHorarios
                          {
                              Sala = "Sala " + s.Key.Formato.Nombre,
                              Horarios = s.Select(h => new HorarioProyeccion
                              {
                                  IdProyeccion = h.Id_Proyeccion,
                                  Hora = h.Hora.ToString(@"hh\:mm")
                              }).OrderBy(h => h.Hora).ToList()
                          }).ToList()
                );

            var vm = new FuncionesViewModel
            {
                Cine = cine,
                Pelicula = pelicula,
                FechasDisponibles = fechas,
                FechaSeleccionada = fechaSeleccionada,
                ProyeccionesPorFecha = proyeccionesPorFecha
            };

            if (desdeVenta)
            {
                Console.WriteLine("🟢 Restaurando datos desde VentaEntradas...");
                ViewBag.DesdeVenta = true;
                ViewBag.FechaSeleccionada = TempData["FechaSeleccionada"];
                ViewBag.HoraSeleccionada = TempData["HoraSeleccionada"];
                ViewBag.SalaSeleccionada = TempData["SalaSeleccionada"];

                // 🔥 Importante: limpiamos todo para cortar la cadena
                TempData.Clear();

                Console.WriteLine("✅ TempData limpiado tras restauración.");
            }
            else
            {
                Console.WriteLine("⚪ Carga directa sin restauración.");
                ViewBag.DesdeVenta = false;
            }

            return View(vm);
        }

        [HttpGet]
        public IActionResult VolverDesdeVentaEntradas()
        {
            Console.WriteLine("🔙 Entró a VolverDesdeVentaEntradas");
            if (!TempData.ContainsKey("CineId") || !TempData.ContainsKey("PeliculaId"))
            {
                Console.WriteLine("❌ No se encontraron datos en TempData. Volviendo a Home.");
                return RedirectToAction("Index", "Home");
            }

            int cineId = Convert.ToInt32(TempData["CineId"]);
            int peliculaId = Convert.ToInt32(TempData["PeliculaId"]);

            Console.WriteLine($"🎬 Redirigiendo a Funciones con cineId={cineId}, peliculaId={peliculaId}");

            TempData["DesdeVentaEntradas"] = true;
            TempData.Keep();

            return RedirectToAction("Index", new { cineId, peliculaId });
        }
    }
}
