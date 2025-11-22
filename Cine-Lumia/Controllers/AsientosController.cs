using Cine_Lumia.Models;
using Cine_Lumia.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Cine_Lumia.Controllers
{
    [Authorize]
    public class AsientosController : Controller
    {
        private readonly CineDbContext _context;

        public AsientosController(CineDbContext context)
        {
            _context = context;
        }

        // ---------------------- PRIMER INGRESO -------------------------
        [Authorize]
        [HttpPost]
        public IActionResult Index(int idProyeccion, int cantidadEntradas, decimal totalCompra, string formatoEntrada)
        {
            TempData["IdProyeccionSeleccionado"] = idProyeccion;
            TempData["CantidadEntradas"] = cantidadEntradas.ToString();
            TempData["FormatoEntrada"] = formatoEntrada;
            TempData["TotalCompra"] = totalCompra.ToString(CultureInfo.InvariantCulture);

            // No guardamos asientos todavía
            TempData.Remove("Asientos");

            TempData.Keep();

            return RedirectToAction("Seleccion");
        }

        // ---------------------- GET SELECCIÓN DE ASIENTOS -------------------------
        [HttpGet]
        public IActionResult Seleccion()
        {

            // Solo preseleccionamos si TempData["Asientos"] existe (viene de Pago)
            List<int> asientosSeleccionadosIds = new List<int>();
            if (TempData.ContainsKey("Asientos"))
            {
                var asientosStr = TempData["Asientos"]?.ToString() ?? "";
                asientosSeleccionadosIds = asientosStr
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList();
            }

            ViewData["AsientosSeleccionados"] = string.Join(",", asientosSeleccionadosIds);

            // Validación básica
            if (!TempData.ContainsKey("IdProyeccionSeleccionado") ||
                !TempData.ContainsKey("CantidadEntradas") ||
                !TempData.ContainsKey("TotalCompra") ||
                !TempData.ContainsKey("FormatoEntrada"))
            {
                return RedirectToAction("Index", "VentaEntradas"); // fallback
            }

            int idProyeccion = int.Parse(TempData["IdProyeccionSeleccionado"].ToString()!);
            int cantidadEntradas = int.Parse(TempData["CantidadEntradas"].ToString()!);
            decimal totalCompra = decimal.Parse(TempData["TotalCompra"].ToString()!, CultureInfo.InvariantCulture);
            string formatoEntrada = TempData["FormatoEntrada"].ToString()!;

            var proyeccion = _context.Proyecciones
                .Include(p => p.Pelicula)
                .Include(p => p.Sala).ThenInclude(s => s.Asientos)
                .Include(p => p.Sala).ThenInclude(s => s.Cine)
                .Include(p => p.Sala).ThenInclude(s => s.Formato)
                .FirstOrDefault(p => p.Id_Proyeccion == idProyeccion);

            if (proyeccion == null) return NotFound();

            var ocupados = _context.Entradas
                .Where(e => e.Id_Proyeccion == idProyeccion)
                .Select(e => e.Id_Asiento)
                .ToList();

            var asientosList = proyeccion.Sala.Asientos
                .Select(a => new AsientoViewModel
                {
                    Id_Asiento = a.Id_Asiento,
                    Fila = a.Fila,
                    Columna = a.Columna,
                    Disponible = !ocupados.Contains(a.Id_Asiento),
                    Seleccionado = asientosSeleccionadosIds.Contains(a.Id_Asiento)
                }).ToList();

            var vm = new SeleccionAsientoViewModel
            {
                Id_Proyeccion = idProyeccion,
                Proyeccion = proyeccion,
                Asientos = asientosList,
                Columnas = proyeccion.Sala.Cant_Butacas,
                Filas = proyeccion.Sala.Cant_Filas,
                CantidadEntradas = cantidadEntradas,
                FormatoEntrada = formatoEntrada,
                TotalCompra = totalCompra
            };

            // Mantener TempData importante
            TempData.Keep("IdProyeccionSeleccionado");
            TempData.Keep("CantidadEntradas");
            TempData.Keep("TotalCompra");
            TempData.Keep("FormatoEntrada");
            TempData.Keep("Asientos");
            // Si existe PagoData, volver a mantenerlo
            if (TempData.ContainsKey("PagoData"))
            {
                var pagoData = TempData["PagoData"];
                TempData["PagoData"] = pagoData; // se vuelve a guardar
                TempData.Keep("PagoData");
            }

            return View("Index", vm);
        }

        // ---------------------- CONFIRMAR SELECCIÓN -------------------------
        [HttpPost]
        public IActionResult ConfirmarSeleccion(int[] asientosSeleccionados)
        {
            if (TempData["CantidadEntradas"] == null || TempData["TotalCompra"] == null || TempData["FormatoEntrada"] == null)
                return RedirectToAction("Index", "Home");

            int cantidad = int.Parse(TempData["CantidadEntradas"].ToString()!);
            string formato = TempData["FormatoEntrada"].ToString()!;
            decimal total = decimal.Parse(TempData["TotalCompra"].ToString()!, CultureInfo.InvariantCulture);
            int idProyeccion = int.Parse(TempData["IdProyeccionSeleccionado"].ToString()!);

            var proyeccion = _context.Proyecciones
                .Include(p => p.Pelicula)
                .Include(p => p.Sala).ThenInclude(s => s.Cine)
                .FirstOrDefault(p => p.Id_Proyeccion == idProyeccion);

            if (proyeccion == null) return NotFound();

            TempData["IdProyeccion"] = idProyeccion.ToString();
            TempData["CantidadEntradas"] = cantidad.ToString();
            TempData["TotalCompra"] = total.ToString(CultureInfo.InvariantCulture);
            TempData["FormatoEntrada"] = formato;
            TempData["Asientos"] = string.Join(",", asientosSeleccionados);

            TempData.Keep();
            TempData.Keep("IdProyeccionSeleccionado");
            TempData.Keep("CantidadEntradas");
            TempData.Keep("TotalCompra");
            TempData.Keep("FormatoEntrada");
            TempData.Keep("Asientos");
            TempData.Keep("PagoData");
            return RedirectToAction("EntradaSnaks", "Consumibles");
        }

        // ---------------------- VOLVER A VENTA DE ENTRADAS -------------------------
        [HttpPost]
        public IActionResult VolverVentaEntradas()
        {
            // Guardar solo lo necesario para prellenar VentaEntradas
            TempData.Keep("CantidadEntradas");
            TempData.Keep("TotalCompra");
            TempData.Keep("FormatoEntrada");
            TempData.Keep("IdProyeccionSeleccionado");

            // Limpiar el carrito de snacks al volver a la selección de cantidad de entradas
            HttpContext.Session.Remove("CarritoSnacks");

            // NO tocamos "Asientos", así se borran si venís de Funciones
            return RedirectToAction("Index", "VentaEntradas");
        }
    }
}
