using Cine_Lumia.Models;
using Cine_Lumia.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cine_Lumia.Controllers
{
    public class AsientosController : Controller
    {
        private readonly CineDbContext _context;
        public AsientosController(CineDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Seleccionar(int idProyeccion)
        {
            var proyeccion = await _context.Proyecciones
                .Include(p => p.Sala)
                .ThenInclude(s => s.Asientos)
                .FirstOrDefaultAsync(p => p.Id_Proyeccion == idProyeccion);

            if (proyeccion == null)
                return NotFound();

            var entradas = await _context.Entradas
                .Where(e => e.Id_Proyeccion == idProyeccion)
                .Select(e => e.Id_Asiento)
                .ToListAsync();

            var vm = new SeleccionAsientosViewModel
            {
                IdProyeccion = idProyeccion,
                SalaNombre = "Sala " + proyeccion.Sala.Formato,
                Asientos = proyeccion.Sala.Asientos
                    .OrderBy(a => a.Fila).ThenBy(a => a.Columna)
                    .Select(a => new AsientoVM
                    {
                        IdAsiento = a.Id_Asiento,
                        Fila = a.Fila,
                        Columna = a.Columna,
                        Ocupado = entradas.Contains(a.Id_Asiento)
                    })
                    .ToList()
            };

            return View(vm);
        }
    }
}
