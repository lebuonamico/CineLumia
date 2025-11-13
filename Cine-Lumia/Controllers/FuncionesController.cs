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
            if (!desdeVenta)
                TempData.Clear();

            var fechas = Enumerable.Range(0, 7)
                .Select(d => DateTime.Today.AddDays(d).ToString("yyyy-MM-dd"))
                .ToList();

            string fechaSeleccionada = fecha ?? DateTime.Today.ToString("yyyy-MM-dd");
            var ahora = DateTime.Now;

            // 🔹 Cargamos todas las proyecciones con sus asientos
            var proyeccionesList = await _context.Proyecciones
                .Include(p => p.Sala)
                    .ThenInclude(s => s.Formato)
                .Include(p => p.Sala.Asientos)
                .Where(p => p.Id_Pelicula == peliculaId && p.Sala.Id_Cine == cineId)
                .ToListAsync();

            // 🔹 Calculamos la disponibilidad de cada proyección
            var proyeccionesConDisponibilidad = new List<(Entities.Proyeccion, int disponibles, int total)>();

            foreach (var p in proyeccionesList)
            {
                int totalAsientos = p.Sala.Asientos.Count();
                int ocupados = await _context.Entradas.CountAsync(e => e.Id_Proyeccion == p.Id_Proyeccion);
                int disponibles = totalAsientos - ocupados;
                proyeccionesConDisponibilidad.Add((p, disponibles, totalAsientos));
            }

            // 🔹 Agrupamos por fecha y sala
            var proyeccionesPorFecha = proyeccionesConDisponibilidad
                .Where(p => p.Item1.Fecha.Date > DateTime.Today ||
                            (p.Item1.Fecha.Date == DateTime.Today && p.Item1.Hora > ahora.TimeOfDay))
                .GroupBy(p => p.Item1.Fecha.Date.ToString("yyyy-MM-dd"))
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(p => p.Item1.Sala)
                          .Select(s => new SalaConHorarios
                          {
                              Sala = "Sala " + s.Key.Formato.Nombre,
                              Horarios = s.Select(h => new HorarioProyeccion
                              {
                                  IdProyeccion = h.Item1.Id_Proyeccion,
                                  Hora = h.Item1.Hora.ToString(@"hh\:mm"),
                                  Disponibles = h.disponibles,
                                  TotalAsientos = h.total
                              }).OrderBy(h => h.Hora).ToList()
                          }).ToList()
                );

            // 🔹 ViewModel final
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
                ViewBag.DesdeVenta = true;
                ViewBag.FechaSeleccionada = TempData["FechaSeleccionada"];
                ViewBag.HoraSeleccionada = TempData["HoraSeleccionada"];
                ViewBag.SalaSeleccionada = TempData["SalaSeleccionada"];
                TempData.Clear();
            }
            else
            {
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
