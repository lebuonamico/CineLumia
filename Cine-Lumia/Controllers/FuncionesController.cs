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

        // GET: Index optionally receives cineId and peliculaId. If TempData/Session contain selection, prefer them to avoid URL tampering.
        public async Task<IActionResult> Index(int? cineId, int? peliculaId, string? fecha)
        {
            // Priorizar TempData (valores guardados por la aplicación)
            if (TempData.ContainsKey("CineId") && TempData.ContainsKey("PeliculaId"))
            {
                var tempCine = Convert.ToInt32(TempData.Peek("CineId"));
                var tempPelicula = Convert.ToInt32(TempData.Peek("PeliculaId"));

                cineId = tempCine;
                peliculaId = tempPelicula;

                // Mantener para próximas peticiones
                TempData.Keep("CineId");
                TempData.Keep("PeliculaId");
            }
            else
            {
                // Si no hay TempData, intentar Session
                var fromSession = HttpContext.Session.GetInt32("CineSeleccionado");
                if (fromSession.HasValue)
                {
                    cineId = fromSession.Value;
                    // intentar recuperar peliculaId desde TempData si existe
                    if (TempData.ContainsKey("PeliculaId"))
                    {
                        peliculaId = Convert.ToInt32(TempData.Peek("PeliculaId"));
                        TempData.Keep("PeliculaId");
                    }
                }
            }

            if (!cineId.HasValue || !peliculaId.HasValue)
            {
                // Como fallback final, usar querystring si no hay selección server-side
                // Esto permite enlaces directos pero no prioriza parámetros pasados manualmente.
                // Si se desea evitar completamente la posibilidad de que el usuario manipule la URL,
                // se puede requerir siempre TempData/Session y devolver error si no existen.
                // Por ahora, si no hay selección en servidor, intentamos tomar los valores de la query.
                if (!cineId.HasValue || !peliculaId.HasValue)
                {
                    // No hay selección válida
                    TempData["ErrorSeleccionCine"] = "Debe seleccionar un cine para ver detalles.";
                    return RedirectToAction("Index", "Home");
                }
            }

            int cid = cineId.Value;
            int pid = peliculaId.Value;

            var cine = await _context.Cines.FirstOrDefaultAsync(c => c.Id_Cine == cid);
            var pelicula = await _context.Peliculas.FirstOrDefaultAsync(p => p.Id_Pelicula == pid);

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
                .Where(p => p.Id_Pelicula == pid && p.Sala.Id_Cine == cid)
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

            Console.WriteLine($"🎬 Guardando TempData y redirigiendo a Funciones sin exponer parámetros.");

            // Mantener los valores en TempData y redirigir a Index sin query string
            TempData["CineId"] = cineId;
            TempData["PeliculaId"] = peliculaId;
            TempData["DesdeVentaEntradas"] = true;
            TempData.Keep();

            return RedirectToAction("Index");
        }
    }
}
