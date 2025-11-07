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

            // Fechas dinámicas: hoy + próximos 6 días
            var fechas = Enumerable.Range(0, 7)
                .Select(d => DateTime.Today.AddDays(d).ToString("yyyy-MM-dd"))
                .ToList();

            string fechaSeleccionada = fecha ?? DateTime.Today.ToString("yyyy-MM-dd");

            // Fecha y hora actual
            var ahora = DateTime.Now;

            // Consulta base
            var proyeccionesQuery = _context.Proyecciones
                .Include(p => p.Sala)
                    .ThenInclude(s => s.Formato)
                .Where(p => p.Id_Pelicula == peliculaId && p.Sala.Id_Cine == cineId);

            var proyeccionesList = await proyeccionesQuery.ToListAsync();

            // 🔹 Filtrar horarios pasados solo si la fecha es hoy
            proyeccionesList = proyeccionesList
                .Where(p =>
                    p.Fecha.Date > DateTime.Today ||
                    (p.Fecha.Date == DateTime.Today && p.Hora > ahora.TimeOfDay)
                )
                .ToList();

            // Agrupar por fecha y sala
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

            return View(vm);
        }
    }
}
