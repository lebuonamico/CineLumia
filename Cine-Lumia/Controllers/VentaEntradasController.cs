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
        public VentaEntradasController(CineDbContext context)
        {
            _context = context;
        }
        [Authorize]
        [Authorize]
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

            // ✅ Caso 1: si viene desde Funciones → limpiar TempData y dejar 1 entrada ya calculada
            if (TempData.ContainsKey("DesdeFunciones"))
            {
                TempData.Remove("DesdeFunciones");

                var tipo = tiposEntrada.First();
                int cantidad = 1;
                decimal total = tipo.Precio;

                vm.Total = total;
                vm.CantidadesSeleccionadas = new Dictionary<int, int> { { tipo.Id_TipoEntrada, cantidad } };

                // Guardamos también en TempData para que el JS los lea correctamente
                TempData["CantidadEntradas"] = cantidad;
                TempData["TotalCompra"] = total.ToString(CultureInfo.InvariantCulture);
                TempData["FormatoEntrada"] = tipo.Formato.Nombre;

                ViewBag.CantidadEntradas = cantidad;
                ViewBag.TotalCompra = total;
                ViewBag.FormatoEntrada = tipo.Formato.Nombre;
            }
            // ✅ Caso 2: si viene de volver desde Selección de Asiento → restaurar valores guardados
            else if (TempData.ContainsKey("CantidadEntradas"))
            {
                int cantidad = int.Parse(TempData["CantidadEntradas"].ToString()!);
                decimal total = decimal.Parse(TempData["TotalCompra"].ToString()!, CultureInfo.InvariantCulture);
                string formato = TempData["FormatoEntrada"].ToString()!;

                vm.Total = total;
                vm.CantidadesSeleccionadas = new Dictionary<int, int> { { tiposEntrada.First().Id_TipoEntrada, cantidad } };

                ViewBag.CantidadEntradas = cantidad;
                ViewBag.TotalCompra = total;
                ViewBag.FormatoEntrada = formato;

                TempData.Keep();
            }

            return View(vm);
        }


        [HttpPost]
        public IActionResult GuardarTempData(int idProyeccion, int cantidadEntradas, decimal totalCompra, string formatoEntrada)
        {
            TempData["CantidadEntradas"] = cantidadEntradas;
            TempData["TotalCompra"] = totalCompra.ToString(CultureInfo.InvariantCulture);
            TempData["FormatoEntrada"] = formatoEntrada;

            // redirige a Index GET limpio (sin datos en URL)
            return RedirectToAction("Index", new { idProyeccion });
        }
        [HttpPost]
        public IActionResult DesdeFunciones(int idProyeccion, string fechaSeleccionada, string horaSeleccionada)
        {
            TempData["DesdeFunciones"] = true;
            TempData["FechaSeleccionada"] = fechaSeleccionada;
            TempData["HoraSeleccionada"] = horaSeleccionada;

            // 🔒 Si no está autenticado, ir al login y luego volver a la venta
            if (!User.Identity.IsAuthenticated)
            {
                var returnUrl = Url.Action("Index", "VentaEntradas", new { idProyeccion });
                return RedirectToAction("Login", "Account", new { returnUrl });
            }

            // ✅ Si ya está logeado, ir directo a la venta
            return RedirectToAction("Index", new { idProyeccion });
        }



        [HttpPost]
        [HttpPost]
        public IActionResult VolverAFunciones(
    int idProyeccion,
    int cantidadEntradas,
    decimal totalCompra,
    string formatoEntrada,
    string fechaSeleccionada,
    string horaSeleccionada)
        {
            // ✅ Guardar datos de la compra
            TempData["CantidadEntradas"] = cantidadEntradas;
            TempData["TotalCompra"] = totalCompra.ToString(CultureInfo.InvariantCulture);
            TempData["FormatoEntrada"] = formatoEntrada;

            // ✅ Guardar datos de navegación
            TempData["DesdeVentaEntradas"] = true;
            TempData["FechaSeleccionada"] = fechaSeleccionada;
            TempData["HoraSeleccionada"] = horaSeleccionada;

            var proyeccion = _context.Proyecciones
                .Include(p => p.Sala)
                .ThenInclude(s => s.Formato)
                .FirstOrDefault(p => p.Id_Proyeccion == idProyeccion);

            if (proyeccion == null)
                return NotFound();

            TempData["CineId"] = proyeccion.Sala.Id_Cine;
            TempData["PeliculaId"] = proyeccion.Id_Pelicula;
            TempData["SalaSeleccionada"] = "Sala " + proyeccion.Sala.Formato.Nombre;

            // 🔹 Volvemos a Funciones sin pasar valores por URL
            return RedirectToAction("VolverDesdeVentaEntradas", "Funciones");
        }



        [HttpGet]
        public IActionResult VolverDesdeVentaEntradas()
        {
            if (!TempData.ContainsKey("CineId") || !TempData.ContainsKey("PeliculaId"))
                return RedirectToAction("Index", "Home");

            int cineId = Convert.ToInt32(TempData["CineId"]);
            int peliculaId = Convert.ToInt32(TempData["PeliculaId"]);

            TempData["DesdeVentaEntradas"] = true;

            // Mantener valores en TempData y redirigir a Funciones para que consuma TempData sin exponer query string
            TempData.Keep();
            return RedirectToAction("VolverDesdeVentaEntradas", "Funciones");
        }


    }
}
