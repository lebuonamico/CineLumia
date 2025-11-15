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

        public async Task<IActionResult> Index(int? cineId, int? peliculaId, string? fecha)
        {
            // Priorizar TempData
            if (TempData.ContainsKey("CineId") && TempData.ContainsKey("PeliculaId"))
            {
                cineId = Convert.ToInt32(TempData.Peek("CineId"));
                peliculaId = Convert.ToInt32(TempData.Peek("PeliculaId"));
            }
            else
            {
                var fromSession = HttpContext.Session.GetInt32("CineSeleccionado");
                if (fromSession.HasValue)
                    cineId = fromSession.Value;
            }

            if (!cineId.HasValue || !peliculaId.HasValue)
            {
                TempData["ErrorSeleccionCine"] = "Debe seleccionar un cine para ver detalles.";
                return RedirectToAction("Index", "Home");
            }

            var cine = await _context.Cines.FirstOrDefaultAsync(c => c.Id_Cine == cineId.Value);
            var pelicula = await _context.Peliculas.FirstOrDefaultAsync(p => p.Id_Pelicula == peliculaId.Value);
            if (cine == null || pelicula == null)
                return NotFound();

            bool desdeVenta = TempData.Peek("DesdeVentaEntradas") != null;
            if (!desdeVenta)
                TempData.Remove("ErrorSeleccionCine"); // solo eliminar errores temporales

            var fechas = Enumerable.Range(0, 7)
                .Select(d => DateTime.Today.AddDays(d).ToString("yyyy-MM-dd"))
                .ToList();

            string fechaSeleccionada = fecha ?? DateTime.Today.ToString("yyyy-MM-dd");
            var ahora = DateTime.Now;

            var proyeccionesList = await _context.Proyecciones
                .Include(p => p.Sala).ThenInclude(s => s.Formato)
                .Include(p => p.Sala.Asientos)
                .Where(p => p.Id_Pelicula == peliculaId.Value && p.Sala.Id_Cine == cineId.Value)
                .ToListAsync();

            var proyeccionesConDisponibilidad = new List<(Entities.Proyeccion, int disponibles, int total)>();
            foreach (var p in proyeccionesList)
            {
                int totalAsientos = p.Sala.Asientos.Count();
                int ocupados = await _context.Entradas.CountAsync(e => e.Id_Proyeccion == p.Id_Proyeccion);
                proyeccionesConDisponibilidad.Add((p, totalAsientos - ocupados, totalAsientos));
            }

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
            }
            else
            {
                ViewBag.DesdeVenta = false;
            }

            // mantener todos los TempData importantes
            TempData.Keep();

            return View(vm);
        }

        [HttpGet]
        public IActionResult VolverDesdeVentaEntradas()
        {
            if (!TempData.ContainsKey("CineId") || !TempData.ContainsKey("PeliculaId"))
                return RedirectToAction("Index", "Home");

            TempData["DesdeVentaEntradas"] = true;
            TempData.Keep();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult SeleccionarProyeccionOculta(int idProyeccion)
        {
            TempData["IdProyeccionSeleccionado"] = idProyeccion;
            TempData.Keep();
            return RedirectToAction("Index", "VentaEntradas");
        }
    }
}
