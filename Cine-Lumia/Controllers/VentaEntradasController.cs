using Cine_Lumia.Models;
using Cine_Lumia.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Cine_Lumia.Controllers
{
    [Authorize]
    public class VentaEntradasController : Controller
    {
        private readonly CineDbContext _context;
        public VentaEntradasController(CineDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!TempData.ContainsKey("IdProyeccionSeleccionado") || TempData["IdProyeccionSeleccionado"] == null)
                return RedirectToAction("Index", "Funciones");

            int idProyeccion = (int)TempData["IdProyeccionSeleccionado"]!;

            var proyeccion = await _context.Proyecciones
                .Include(p => p.Sala).ThenInclude(s => s.Formato)
                .Include(p => p.Pelicula)
                .Include(p => p.Sala.Cine)
                .FirstOrDefaultAsync(p => p.Id_Proyeccion == idProyeccion);

            if (proyeccion == null) return NotFound();

            var tiposEntrada = await _context.TipoEntrada
                .Where(t => t.Id_Formato == proyeccion.Sala.Id_Formato)
                .ToListAsync();

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

            // --- Lógica de inicialización ---
            bool desdeFunciones = TempData.ContainsKey("DesdeFunciones");

            if (desdeFunciones)
            {
                // Inicializamos siempre en 1 si viene de funciones
                var tipo = tiposEntrada.First();
                vm.Total = tipo.Precio;
                vm.CantidadesSeleccionadas = new Dictionary<int, int> { { tipo.Id_TipoEntrada, 1 } };

                // Limpiamos todos los valores previos que podrían existir
                TempData.Remove("CantidadEntradas");
                TempData.Remove("TotalCompra");
                TempData.Remove("FormatoEntrada");
                TempData.Remove("DesdeFunciones"); // eliminamos la bandera para futuras visitas
            }
            else if (TempData.ContainsKey("CantidadEntradas") && TempData["CantidadEntradas"] != null && TempData.ContainsKey("TotalCompra") && TempData["TotalCompra"] != null)
            {
                // Viene de Selección de Asientos o regreso a VentaEntradas → conservamos los valores
                vm.Total = decimal.Parse(TempData["TotalCompra"]!.ToString()!, CultureInfo.InvariantCulture);
                vm.CantidadesSeleccionadas = new Dictionary<int, int>
        {
            { tiposEntrada.First().Id_TipoEntrada, int.Parse(TempData["CantidadEntradas"]!.ToString()!) }
        };
            }
            else
            {
                // Fallback: inicializamos en 1
                var tipo = tiposEntrada.First();
                vm.Total = tipo.Precio;
                vm.CantidadesSeleccionadas = new Dictionary<int, int> { { tipo.Id_TipoEntrada, 1 } };
            }

            // Pasamos los valores al JS mediante ViewBag
            ViewBag.CantidadEntradas = vm.CantidadesSeleccionadas.First().Value;
            ViewBag.TotalCompra = vm.Total;
            ViewBag.FormatoEntrada = tiposEntrada.First().Formato.Nombre;

            TempData.Keep();
            return View(vm);
        }





        [HttpPost]
        public IActionResult GuardarTempData(int idProyeccion, int cantidadEntradas, decimal totalCompra, string formatoEntrada)
        {
            TempData["CantidadEntradas"] = cantidadEntradas;
            TempData["TotalCompra"] = totalCompra.ToString(CultureInfo.InvariantCulture);
            TempData["FormatoEntrada"] = formatoEntrada;
            TempData.Keep();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DesdeFunciones(int idProyeccion, string fechaSeleccionada, string horaSeleccionada)
        {
            TempData["DesdeFunciones"] = true;
            TempData["FechaSeleccionada"] = fechaSeleccionada;
            TempData["HoraSeleccionada"] = horaSeleccionada;
            TempData.Keep();

            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                var returnUrl = Url.Action("Index", "VentaEntradas", new { idProyeccion });
                return RedirectToAction("Login", "Account", new { returnUrl });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult VolverAFunciones(int idProyeccion, int cantidadEntradas, decimal totalCompra, string formatoEntrada, string fechaSeleccionada, string horaSeleccionada)
        {
            TempData["IdProyeccionSeleccionado"] = idProyeccion;
            TempData["CantidadEntradas"] = 1;
            TempData["TotalCompra"] = totalCompra.ToString(CultureInfo.InvariantCulture);
            TempData["FormatoEntrada"] = formatoEntrada;
            TempData["DesdeVentaEntradas"] = true;
            TempData["FechaSeleccionada"] = fechaSeleccionada;
            TempData["HoraSeleccionada"] = horaSeleccionada;

            var proyeccion = _context.Proyecciones
                .Include(p => p.Sala).ThenInclude(s => s.Formato)
                .FirstOrDefault(p => p.Id_Proyeccion == idProyeccion);

            if (proyeccion == null) return NotFound();

            TempData["CineId"] = proyeccion.Sala.Id_Cine;
            TempData["PeliculaId"] = proyeccion.Id_Pelicula;
            TempData["SalaSeleccionada"] = "Sala " + proyeccion.Sala.Formato.Nombre;

            TempData.Keep();
            return RedirectToAction("VolverDesdeVentaEntradas", "Funciones");
        }
    }
}
